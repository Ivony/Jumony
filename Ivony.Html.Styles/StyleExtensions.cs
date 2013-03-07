using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html
{
  public static class StyleExtensions
  {

    public static readonly string styleClassExpressionPattern = @"\.(?<className>\w+);";
    public static readonly string styleExpressionPattern = string.Format( @"(\s*(?<class>{0})|(?<setting>{1}))+\s*", styleClassExpressionPattern, Regulars.styleSettingPattern );

    private static readonly Regex styleExpressionRegex = new Regex( "^" + styleExpressionPattern + "$", RegexOptions.Compiled );

    /// <summary>
    /// 对元素设置指定样式
    /// </summary>
    /// <typeparam name="T">元素实例类型</typeparam>
    /// <param name="element">要设置样式的元素</param>
    /// <param name="styleExpression">样式表达式</param>
    /// <returns>设置了样式的元素</returns>
    public static T Style<T>( this T element, string styleExpression ) where T : IHtmlElement
    {

      var match = styleExpressionRegex.Match( styleExpression );
      if ( !match.Success )
        throw new FormatException();

      var style = element.Style();

      foreach ( Capture capture in match.Groups["class"].Captures )
      {
        style.AddClass( capture.FindCaptures( match.Groups["className"] ).Single().Value );
      }

      foreach ( Capture capture in match.Groups["setting"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).Single().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).Single().Value;

        style.SetValue( name, value );
      }


      return element;
    }

  }
}
