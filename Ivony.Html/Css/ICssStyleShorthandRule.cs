using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Css
{

  /// <summary>
  /// 定义 CSS 样式设置缩写规则
  /// </summary>
  public interface ICssStyleShorthandRule
  {

    /// <summary>
    /// 属性名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 解出属性值
    /// </summary>
    /// <param name="value">属性值缩写形式</param>
    /// <returns>解出的属性值</returns>
    public CssStyleProperty[] ExtractProperties( string shorthand );

  }


  public class PaddingStyleShorthandRule : ICssStyleShorthandRule
  {
    public string Name
    {
      get { return "padding"; }
    }


    private Regex whitespaceRegex = new Regex( @"\s+", RegexOptions.Compiled );

    public CssStyleProperty[] ExtractProperties( string shorthand )
    {
      var values = whitespaceRegex.Split( shorthand );

      string top, right, bottom, left;

      if ( values.Length == 0 )
        return new CssStyleProperty[0];

      top = right = bottom = left = values[0];

      if ( values.Length >= 2 )
        right = left = values[1];

      if ( values.Length >= 3 )
        bottom = values[2];

      if ( values.Length >= 4 )
        left = values[3];


      return new[] { new CssStyleProperty( "padding-top", top ), new CssStyleProperty( "padding-right", right ), new CssStyleProperty( "padding-bottom", bottom ), new CssStyleProperty( "padding-left", left ) };
    }
  }


}
