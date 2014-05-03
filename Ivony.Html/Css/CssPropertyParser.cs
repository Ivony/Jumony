using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html
{

  /// <summary>
  /// CSS 样式解析器
  /// </summary>
  public static class CssPropertyParser
  {


    private static string propertyDeclarationPattern = @"(?<=^|;)\s*(?<name>(?>(?![0-9-])[\-_\w]+))(?>\s*):(?>\s*)(?<value>.*?)(?<important>!important)?(;|$)";

    private static Regex propertyDeclarationRegex = new Regex( propertyDeclarationPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );

    //private static string nonasciiPattern = @"[^\0-\237]";
    private static string unicodePattern = @"\\[0-9a-f]{1,6}(\r\n|[ \n\r\t\f])?";
    private static string escapePattern = "{unicode}|\\[^\n\r\f0-9a-f]".Replace( "{unicode}", "(?:" + unicodePattern + ")" );


    /// <summary>
    /// 解析 CSS 样式属性
    /// </summary>
    /// <param name="expression">要解析的 CSS 样式表达式</param>
    /// <returns>CSS 样式属性</returns>
    public static CssStyleProperty[] ParseProperties( string expression )
    {
      return propertyDeclarationRegex.Matches( expression ?? "" ).Cast<Match>().Select(
        match => new CssStyleProperty( match.Groups["name"].Value, match.Groups["value"].Value, match.Groups["important"].Success )
      ).ToArray();
    }


    /// <summary>
    /// 解析 CSS 样式属性
    /// </summary>
    /// <param name="styleExpression">CSS 样式设置表达式</param>
    /// <param name="specification">所需要采用的 CSS 规范</param>
    /// <returns>CSS 样式属性</returns>
    public static CssStyle ParseCssStyle( string styleExpression, CssStyleSpecificationBase specification = null )
    {

      var style = new CssStyle( specification ?? new Css21StyleSpecification() );
      style.SetProperties( ParseProperties( styleExpression ) );
      return style;
    }


  }
}
