using Ivony.Html.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// 提供操纵元素 CSS 样式的一些扩展方法
  /// </summary>
  public static class StyleExtensions
  {

    /// <summary>
    /// 获取元素的样式对象，用于方便的操纵元素样式
    /// </summary>
    /// <param name="element">要操纵样式的元素</param>
    /// <returns>样式对象</returns>
    public static StyleManager Style( this IHtmlElement element )
    {

      return StyleManager.GetStyleManager( element );
    }





    /// <summary>
    /// 对元素设置指定样式
    /// </summary>
    /// <typeparam name="T">元素实例类型</typeparam>
    /// <param name="element">要设置样式的元素</param>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>设置了样式的元素</returns>
    public static T Style<T>( this T element, string name, string value ) where T : IHtmlElement
    {
      var style = element.Style();
      style.SetValue( name, value );

      return element;
    }
  }
}
