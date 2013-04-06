using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html
{
  public static class CssPropertyParser
  {


    private static string propertyDeclarationPattern = @"(?<=^|;)\s*(?<name>(?>(?![0-9-])[\-_\w]+))(?>\s*):(?>\s*)(?<value>.+?)(?<important>!important)?(;|$)";

    private static Regex propertyDeclarationRegex = new Regex( propertyDeclarationPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );

    private static string nonasciiPattern = @"[^\0-\237]";
    private static string unicodePattern = @"\\[0-9a-f]{1,6}(\r\n|[ \n\r\t\f])?";
    private static string escapePattern = "{unicode}|\\[^\n\r\f0-9a-f]".Replace( "{unicode}", "(?:" + unicodePattern + ")" );



    public static CssStyleProperty[] ParseProperties( string expression )
    {
      return propertyDeclarationRegex.Matches( expression ).Cast<Match>().Select(
        match => new CssStyleProperty( match.Groups["name"].Value, match.Groups["value"].Value, match.Groups["important"].Success )
      ).ToArray();
    }


    /// <summary>
    /// 解析 CSS 样式设置
    /// </summary>
    /// <param name="styleExpression">CSS 样式设置表达式</param>
    /// <returns>CSS 样式设置</returns>
    public static CssStyle ParseCssStyle( string styleExpression )
    {
      var style = new CssStyle( new Css21StyleSpecification() );
      style.SetProperties( ParseProperties( styleExpression ) );
      return style;
    }


  }
}
