using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ivony.Html
{

  /// <summary>
  /// CSS层叠选择器
  /// </summary>
  /// <remarks>
  /// 层叠选择器的表达式分析过程是从左至右，而处理则是从右至左，采取从左至右的方式分析主要考虑到正则工作模式和效率问题。但由于处理方式是从右至左，所以左选择器（父级选择器）是可选的，而右选择器（子级选择器）是必须的。
  /// 简单的说只有一个元素选择器所构成的层叠选择器，其元素选择器是位于右边的。
  /// </remarks>
  internal sealed class CssCasecadingSelector : ICssSelector
  {


    public static readonly Regex casecadingSelectorRegex = new Regex( "^" + Regulars.cssCasecadingSelectorPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    public static readonly Regex extraRegex = new Regex( "^" + Regulars.extraExpressionPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );




    /// <summary>
    /// 创建选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>选择器实例</returns>
    public static ICssSelector Create( string expression )
    {
      var match = casecadingSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      ICssSelector selector = CssElementSelector.Create( match.Groups["leftSelector"].Value );

      foreach ( var extraCapture in match.Groups["extra"].Captures.Cast<Capture>() )
      {
        var relative = extraCapture.FindCaptures( match.Groups["relative"] ).Single().Value.Trim();
        var rightSelector = extraCapture.FindCaptures( match.Groups["rightSelector"] ).Single().Value.Trim();

        selector = new CssCasecadingSelector( CssElementSelector.Create( rightSelector ), relative, selector );

      }


      return selector;
    }


    /// <summary>
    /// 创建选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定，上溯时不超出此范畴</param>
    /// <returns>带范畴限定的层叠选择器</returns>
    /// <remarks>
    /// 层叠选择器已经被重写以适应更多情况，范畴限定也已经被包装为一个ICssSelector对象，作为左选择器而存在。所以范畴限定等同于，仅选择这个容器子元素的选择器。
    /// 为此还约定了一个特殊关系运算符：null，这个关系运算符表示被考察的元素本身必须同时满足左选择器。换言之A null .class其实等同于A.class
    /// 在范畴限定的ICssSelector对象实现中，容器的所有子代都会被认为符合条件，从而实现了范畴限定。
    /// </remarks>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {
      var selector = Create( expression );

      if ( scope == null )
        return selector;

      return new CssCasecadingSelector( selector, null, new CssScopeRestrictionSelector( scope ) );

    }



    private readonly string _relative;
    /// <summary>
    /// 关系描述符
    /// </summary>
    public string Relative
    {
      get { return _relative; }
    }

    private readonly ICssSelector _right;
    /// <summary>
    /// 子级选择器
    /// </summary>
    public ICssSelector RightSelector
    {
      get { return _right; }
    }

    private readonly ICssSelector _left;
    /// <summary>
    /// 父级选择器
    /// </summary>
    public ICssSelector LeftSelector
    {
      get { return _left; }
    }



    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="rightSelector">右选择器</param>
    /// <param name="relative">关系选择符</param>
    /// <param name="leftSelector">左选择器</param>
    public CssCasecadingSelector( ICssSelector rightSelector, string relative, ICssSelector leftSelector )
    {
      _right = rightSelector;

      if ( relative != null )
        _relative = relative.Trim();

      _left = leftSelector;
    }



    /// <summary>
    /// 检查元素是否符合选择条件
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否符合选择条件</returns>
    public bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;



      if ( !RightSelector.IsEligible( element ) )
        return false;

      if ( _left == null )
        return true;



      if ( Relative == null )
        return LeftSelector.IsEligible( element );


      else if ( Relative == ">" )
        return LeftSelector.IsEligible( element.Parent() );

      else if ( Relative == "" )
        return element.Ancestors().Any( e => LeftSelector.IsEligible( e ) );

      else if ( Relative == "+" )
        return LeftSelector.IsEligible( element.PreviousElement() );

      else if ( Relative == "~" )
        return element.SiblingsBeforeSelf().Any( e => LeftSelector.IsEligible( e ) );

      else
        throw new NotSupportedException( string.Format( CultureInfo.InvariantCulture, "不支持的关系选择符 \"{0}\"", Relative ) );
    }



    /// <summary>
    /// 重写ToString输出规范化的选择器表达式
    /// </summary>
    /// <returns>选择器表达式</returns>
    public override string ToString()
    {
      if ( Relative == null )
        return RightSelector.ToString();

      else if ( Relative == "" )
        return string.Format( "{0} {1}", LeftSelector, RightSelector );

      else
        return string.Format( "{0} {1} {2}", LeftSelector, Relative, RightSelector );
    }



    private class CssScopeRestrictionSelector : ICssSelector
    {

      private readonly IHtmlContainer _scope;

      public CssScopeRestrictionSelector( IHtmlContainer scope )
      {
        if ( scope == null )
          throw new ArgumentNullException( "scope" );

        _scope = scope;

      }

      public bool IsEligible( IHtmlElement element )
      {

        while ( element != null )
        {
          if ( element.Container.Equals( _scope ) )
            return true;

          element = element.Parent();

        }

        return false;
      }

      public override string ToString()
      {

        IHtmlElement element = _scope as IHtmlElement;
        if ( element != null )
          return "#" + element.Unique() + " ";
        else if ( _scope is IHtmlDocument )
          return "#document# ";
        else if ( _scope is HtmlFragment )
          return "#fragment# ";
        else
          return "#unknow#";

      }

    }

    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="elements">作为范畴限定的元素集</param>
    /// <returns>层叠选择器</returns>
    public static ICssSelector Create( string expression, IEnumerable<IHtmlElement> elements )
    {
      var rightSelector = Create( expression );

      return new CssCasecadingSelector( rightSelector, "", new CssElementsRestrictionSelector( elements ) );
    }


    private class CssElementsRestrictionSelector : ICssSelector
    {

      private readonly IEnumerable<IHtmlElement> _elements;

      public CssElementsRestrictionSelector( IEnumerable<IHtmlElement> elements )
      {

        if ( elements == null )
          throw new ArgumentNullException( "elements" );

        _elements = elements;

      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        return _elements.Contains( element );
      }

      public override string ToString()
      {
        return "#elements#";
      }
    }
  }
}
