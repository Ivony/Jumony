using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Ivony.Fluent;

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



    //选择器缓存
    private static Dictionary<string, ICssSelector> selectorCache = new Dictionary<string, ICssSelector>( StringComparer.Ordinal );
    private static object _cacheSync = new Object();




    /// <summary>
    /// 创建关系选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>选择器实例</returns>
    public static ICssSelector Create( string expression )
    {

      if ( selectorCache.ContainsKey( expression ) )
        return selectorCache[expression];

      var match = casecadingSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();



      ICssSelector rightSelector = CssElementSelector.Create( match.Groups["selector"].Captures.Cast<Capture>().Last().Value.Trim() );
      
      foreach ( var relativeCapture in match.Groups["relativeSelector"].Captures.Cast<Capture>().Reverse() )
      {
        var relative = relativeCapture.FindCaptures( match.Groups["relative"] ).Single().Value.Trim();
        var selector = relativeCapture.FindCaptures( match.Groups["selector"] ).Single().Value.Trim();

        var relativeSelector = CreateRelativeSelector( CssElementSelector.Create( selector ), relative );
        rightSelector = new CssCasecadingSelector( relativeSelector, rightSelector );
      }


      lock ( _cacheSync )
      {
        selectorCache[expression] = rightSelector;
      }

      return rightSelector;
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
    /// 那么，为什么不能使用子代关系符来创建选择器呢？譬如说使得左选择器选择范畴容器，而关系符则是子代""？
    /// 这是因为容器是IHtmlContainer类型，但CSS选择器（左右选择器）要求只能选择IHtmlElement对象。
    /// 当然我们可以使得IHtmlContainer是一个Element，或者使得CSS选择器可以选择IHtmlContainer对象，但无论哪一种，都使得整个选择器模型变得很不自然。
    /// </remarks>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {
      var selector = Create( expression );

      return Create( selector, scope );
    }

    /// <summary>
    /// 创建范畴限定选择器
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <param name="scope">范畴限定，上溯时不超出此范畴</param>
    /// <returns>>带范畴限定的层叠选择器</returns>
    public static ICssSelector Create( ICssSelector selector, IHtmlContainer scope )
    {

      if ( selector == null )
        throw new ArgumentNullException( "selector" );

      if ( scope == null )
        return selector;

      return new CssCasecadingSelector( new CssScopeRestrictionSelector( scope ), selector );
    }




    /// <summary>
    /// 关系选择器
    /// </summary>
    public ICssSelector RelativeSelector
    {
      get;
      private set;
    }

    /// <summary>
    /// 元素选择器
    /// </summary>
    public ICssSelector RightSelector
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="relativeSelector">关系选择器</param>
    /// <param name="rightSelector">元素选择器</param>
    public CssCasecadingSelector( ICssSelector relativeSelector, ICssSelector rightSelector )
    {
      // TODO: Complete member initialization
      RelativeSelector = relativeSelector;
      RightSelector = rightSelector;
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

      return RelativeSelector.IsEligibleBuffered( element );

    }





    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {
      return string.Format( CultureInfo.InvariantCulture, "{0} {1}", RelativeSelector, RightSelector );
    }



    /// <summary>
    /// 定义范畴限定选择器
    /// </summary>
    private class CssScopeRestrictionSelector : ICssSelector
    {

      private readonly IHtmlContainer _scope;


      /// <summary>
      /// 创建范畴限定选择器实例
      /// </summary>
      /// <param name="scope"></param>
      public CssScopeRestrictionSelector( IHtmlContainer scope )
      {
        if ( scope == null )
          throw new ArgumentNullException( "scope" );

        _scope = scope;
      }

      public bool IsEligible( IHtmlElement element )
      {

        if ( element == null )
          return false;

        return element.IsDescendantOf( _scope );
      }

      public override string ToString()
      {

        IHtmlElement element = _scope as IHtmlElement;
        if ( element != null )
          return "#" + element.Unique() + " ";
        else if ( _scope is IHtmlDocument )
          return "#document# ";
        else if ( _scope is IHtmlFragment )
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

      if ( elements.IsNullOrEmpty() )
        return rightSelector;

      return new CssCasecadingSelector( new CssElementsRestrictionSelector( elements ), rightSelector );
    }


    public CssCasecadingSelector Clone()
    {
      throw new NotImplementedException();
    }




    private class CssElementsRestrictionSelector : ICssSelector
    {

      private readonly HashSet<IHtmlElement> _elements;

      public CssElementsRestrictionSelector( IEnumerable<IHtmlElement> elements )
      {

        if ( elements == null )
          throw new ArgumentNullException( "elements" );

        _elements = new HashSet<IHtmlElement>( elements );

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


    /// <summary>
    /// 调用此方法预热选择器
    /// </summary>
    public static void WarmUp()
    {
      casecadingSelectorRegex.IsMatch( "" );
      CssElementSelector.WarmUp();
    }




    /// <summary>
    /// 创建关系选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="relative">关系运算符</param>
    /// <returns>关系选择器</returns>
    private static ICssSelector CreateRelativeSelector( ICssSelector leftSelector, string relative )
    {
      if ( relative == ">" )
        return new ParentRelativeSelector( leftSelector );

      else if ( relative == "" )
        return new AncetorRelativeSelector( leftSelector );

      else if ( relative == "+" )
        return new PreviousRelativeSelector( leftSelector );

      else if ( relative == "~" )
        return new SiblingsRelativeSelector( leftSelector );

      throw new NotSupportedException( "不支持的关系运算符" );
    }

    /// <summary>
    /// 父级关系选择器
    /// </summary>
    private class ParentRelativeSelector : ICssSelector
    {

      private ICssSelector _leftSelector;

      public ParentRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        return _leftSelector.IsEligibleBuffered( element.Parent() );
      }

      public override string ToString()
      {
        return _leftSelector.ToString() + " > ";
      }

    }


    /// <summary>
    /// 父代关系选择器
    /// </summary>
    private class AncetorRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public AncetorRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        var parant = element.Parent();

        //如果父级元素符合左选择器，或者父级元素符合本选择器。
        return _leftSelector.IsEligibleBuffered( parant ) || this.IsEligibleBuffered( parant );
      }


      public override string ToString()
      {
        return _leftSelector.ToString() + " ";
      }
    }


    /// <summary>
    /// 毗邻关系选择器
    /// </summary>
    private class PreviousRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public PreviousRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        return _leftSelector.IsEligibleBuffered( element.PreviousElement() );
      }

      public override string ToString()
      {
        return _leftSelector.ToString() + " + ";
      }
    }


    /// <summary>
    /// 兄弟元素选择器
    /// </summary>
    private class SiblingsRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public SiblingsRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = new PreviousRelativeSelector( leftSelector );
      }

      public bool IsEligible( IHtmlElement element )
      {
        var previous = element.PreviousElement();

        return _leftSelector.IsEligibleBuffered( previous ) || this.IsEligibleBuffered( previous );
      }


      public override string ToString()
      {
        return _leftSelector.ToString() + " ~ ";
      }
    }





  }
}
