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
      return Create( expression, null );
    }




    /// <summary>
    /// 创建一个CSS选择器
    /// </summary>
    /// <param name="scope">范畴限定</param>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    public static ICssSelector Create( string expression, IHtmlContainer scope )
    {

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException( "无法识别的CSS选择器" );


      var selectors = match.Groups["selector"].Captures.Cast<Capture>().Select( c => CssCasecadingSelector.Create( c.Value, scope ) ).ToArray();

      return new CssMultipleSelector( selectors );
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
