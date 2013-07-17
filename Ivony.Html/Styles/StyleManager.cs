using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Ivony.Fluent;
using System.Globalization;


namespace Ivony.Html
{
  using Ivony.Html.Styles;

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


namespace Ivony.Html.Styles
{


  /// <summary>
  /// 提供元素CSS样式管理
  /// </summary>
  public class StyleManager
  {


    private CssStyle _style;

    private string _styleString;

    private IHtmlElement _element;

    private IHtmlAttribute _styleAttribute;


    /// <summary>
    /// 获取指定元素的样式管理器
    /// </summary>
    /// <param name="element">要获取样式管理器的元素</param>
    /// <returns>获取的样式管理器</returns>
    internal static StyleManager GetStyleManager( IHtmlElement element )
    {
      var styleAttribute = element.Attribute( "style" );
      var dataContainer = styleAttribute as IDataContainer;


      //如果 style 属性支持缓存数据容器，则尝试缓存获取。
      if ( dataContainer != null )
      {
        var manager = dataContainer.Data[typeof( StyleManager )] as StyleManager;
        if ( manager != null && manager._styleString.EqualsIgnoreCase( styleAttribute.Value() ) )
          return manager;

        manager = new StyleManager( element );
        dataContainer.Data[typeof( StyleManager )] = manager;
        return manager;
      }

      return new StyleManager( element );
    }



    private StyleManager( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      _element = element;

      lock ( element.SyncRoot )
      {
        _styleAttribute = element.Attribute( "style" );
        _style = CssPropertyParser.ParseCssStyle( _styleAttribute.Value().IfNull( "" ) );
      }
    }



    /// <summary>
    /// 获取样式设置值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public virtual string GetValue( string name )
    {

      EnsureStyle();

      return _style[name];
    }


    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>样式管理器自身</returns>
    public virtual StyleManager SetValue( string name, string value )
    {

      lock ( _element.SyncRoot )
      {

        EnsureStyle();

        _style[name] = value;

        _element.SetAttribute( "style", _style.ToString() );

        return this;
      }
    }



    /// <summary>
    /// 确保当前跟踪的样式是最新的
    /// </summary>
    private void EnsureStyle()
    {
      lock ( _element.SyncRoot )
      {
        var styleAttribute = _element.Attribute( "style" );
        if ( _modified || styleAttribute != _styleAttribute )
        {
          _styleString = styleAttribute.Value().IfNull( "" );
          _style = CssPropertyParser.ParseCssStyle( _styleString );
          _styleAttribute = styleAttribute;
        }
      }
    }





  }
}
