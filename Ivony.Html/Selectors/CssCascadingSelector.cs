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
  internal sealed class CssCasecadingSelector : ICssSelector
  {


    public static readonly Regex cssSelectorRegex = new Regex( "^" + Regulars.cssCasecadingSelectorPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    public static readonly Regex extraRegex = new Regex( "^" + Regulars.extraExpressionPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );




    /// <summary>
    /// 创建选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    public static ICssSelector Create( string expression )
    {
      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      ICssSelector selector = CssElementSelector.Create( match.Groups["elementSelector"].Value );

      foreach ( var extraExpression in match.Groups["extra"].Captures.Cast<Capture>().Select( c => c.Value ) )
      {
        var extraMatch = extraRegex.Match( extraExpression );

        if ( !extraMatch.Success )
          throw new FormatException();

        var relative = extraMatch.Groups["relative"].Value.Trim();
        var elementSelector = extraMatch.Groups["elementSelector"].Value.Trim();

        var newPartSelector = new CssCasecadingSelector( CssElementSelector.Create( elementSelector ), relative, selector );
        selector = newPartSelector;

      }


      return selector;
    }


    /// <summary>
    /// 创建选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定，上溯时不超出此范畴</param>
    /// <returns></returns>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {
      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      var selector = Create( expression );

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


    //没有左选择器的情况
    private CssCasecadingSelector( CssElementSelector selector ) : this( selector, null, null ) { }

    //一般情况
    private CssCasecadingSelector( ICssSelector rightSelector, string relative, ICssSelector leftSelector )
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

        _scope = scope;

      }

      public bool IsEligible( IHtmlElement element )
      {

        while ( element.Container != null )
        {
          if ( element.Container == _scope )
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




  }
}
