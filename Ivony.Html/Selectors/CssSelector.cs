using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using Ivony.Fluent;
using System.Diagnostics;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// 代表一个CSS选择器
  /// </summary>
  public sealed class CssSelector
  {


    private static readonly Regex cssSelectorRegex = new Regex( Regulars.cssSelectorPattern, RegexOptions.Compiled );


    /// <summary>
    /// 创建一个CSS选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    public static ICssSelector Create( string expression )
    {
      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      return Create( expression, null );
    }




    /// <summary>
    /// 创建一个CSS选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定</param>
    /// <returns></returns>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {
      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException( "无法识别的CSS选择器" );


      var selectors = match.Groups["selector"].Captures.Cast<Capture>().Select( c => CssCasecadingSelector.Create( c.Value, scope ) ).ToArray();

      return new CssMultipleSelector( selectors );
    }



    /// <summary>
    /// 执行CSS选择器搜索
    /// </summary>
    /// <param name="expression">CSS选择器表达式</param>
    /// <param name="scope">CSS选择器和搜索范畴</param>
    /// <returns>搜索结果</returns>
    public static IEnumerable<IHtmlElement> Search( string expression, IHtmlContainer scope )
    {

      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );


      var selector = Create( expression, scope );
      return selector.Filter( scope.Descendants() );

    }
  }


  public static class SelectorExtensions
  {

    public static IEnumerable<IHtmlElement> Filter( this ICssSelector selector, IEnumerable<IHtmlElement> source )
    {
      return source.Where( e => selector.IsEligible( e ) );
    }


  }

}
