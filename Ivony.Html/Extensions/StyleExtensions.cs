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


    /// <summary>
    /// 对元素设置指定样式
    /// </summary>
    /// <typeparam name="T">元素实例类型</typeparam>
    /// <param name="element">要设置样式的元素</param>
    /// <param name="properties">要设置的样式集</param>
    /// <returns>设置了样式的元素</returns>
    public static T Style<T>( this T element, IDictionary<string, string> properties ) where T : IHtmlElement
    {
      foreach ( var pair in properties )
        Style( element, pair.Key, pair.Value );

      return element;
    }


    /// <summary>
    /// 获取元素的样式类管理器
    /// </summary>
    /// <param name="element">要获取样式类管理器的元素</param>
    /// <returns>样式类管理器</returns>
    public static StyleClassManager Class( this IHtmlElement element )
    {
      return StyleClassManager.GetStyleClassManager( element );
    }

    /// <summary>
    /// 设置元素的样式类
    /// </summary>
    /// <param name="element">要设置样式类的元素</param>
    /// <param name="classes">要设置的样式类</param>
    /// <returns>被操作的元素</returns>
    public static T Class<T>( this T element, params string[] classes ) where T : IHtmlElement
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( classes == null )
        throw new ArgumentNullException( "classes" );

      classes = classes.SelectMany( i => Regulars.whiteSpaceSeparatorRegex.Split( i ) ).Where( name => name != "" ).ToArray();

      var manager = StyleClassManager.GetStyleClassManager( element );
      foreach ( var expression in classes )
      {

        if ( expression.StartsWith( "-" ) )
          manager.Remove( expression.Substring( 1 ) );

        else if ( expression.StartsWith( "~" ) )
          manager.Toggle( expression.Substring( 1 ) );

        else if ( expression.StartsWith( "+" ) )
          manager.Add( expression.Substring( 1 ) );

        else
          manager.Add( expression );

      }

      return element;
    }

  }
}
