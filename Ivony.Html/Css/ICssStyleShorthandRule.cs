using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 CSS 样式设置缩写规则
  /// </summary>
  public interface ICssStyleShorthandRule
  {

    /// <summary>
    /// 属性名
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 解出属性值
    /// </summary>
    /// <param name="shorthand">属性值缩写形式</param>
    /// <returns>解出的属性值</returns>
    CssStyleProperty[] ExtractProperties( string shorthand );


    /// <summary>
    /// 尝试获取缩写的样式
    /// </summary>
    /// <param name="cssStyle">要检查的样式设置，从中找出可以写成缩写形式的样式</param>
    /// <returns>缩写后的样式</returns>
    CssStyleProperty TryGetShorthandProperty( CssStyle cssStyle );
  }


  internal class StandardBoxShorthandRule : ICssStyleShorthandRule
  {

    public StandardBoxShorthandRule( string propertyName )
    {
      Name = propertyName;
    }

    public string Name
    {
      get;
      private set;
    }

    public CssStyleProperty[] ExtractProperties( string shorthand )
    {
      var values = CssStyleHelper.whitespaceRegex.Split( shorthand );
      return CssStyleHelper.GenerateBoxProperties( Name, values );
    }

    public CssStyleProperty TryGetShorthandProperty( CssStyle cssStyle )
    {
      return null;
    }
  }




  internal static class CssStyleHelper
  {

    public static readonly Regex whitespaceRegex = new Regex( @"\s+", RegexOptions.Compiled );
    public static CssStyleProperty[] GenerateBoxProperties( string prefix, string[] values )
    {
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


      return new[] { new CssStyleProperty( prefix + "-top", top ), new CssStyleProperty( prefix + "-right", right ), new CssStyleProperty( prefix + "-bottom", bottom ), new CssStyleProperty( prefix + "-left", left ) };
    }


    public static readonly Regex lengthValueRegex = new Regex( @"^\d+(px|cm|in)$" );

    public static bool IsLengthValue( string value )
    {
      return lengthValueRegex.IsMatch( value );
    }

    public static readonly Regex colorValueRegex = new Regex( @"^(#([0-9a-fA-F]{3}|[0-9a-fA-F]{6}))$" );

    public static bool IsColorValue( string value )
    {
      return colorValueRegex.IsMatch( value );
    }

  }
}
