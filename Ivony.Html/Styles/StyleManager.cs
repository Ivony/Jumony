using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Ivony.Fluent;
using System.Globalization;


namespace Ivony.Html.Styles
{


  /// <summary>
  /// 提供元素CSS样式管理
  /// </summary>
  public class StyleManager
  {


    private CssStyle _style;

    private IHtmlElement _element;
    private IHtmlAttribute _attribute;


    /// <summary>
    /// 获取指定元素的样式管理器
    /// </summary>
    /// <param name="element">要获取样式管理器的元素</param>
    /// <returns>获取的样式管理器</returns>
    internal static StyleManager GetStyleManager( IHtmlElement element )
    {
      var dataContainer = element as IDataContainer;


      //如果 style 属性支持缓存数据容器，则尝试缓存获取。
      if ( dataContainer != null )
      {
        var manager = dataContainer.Data[typeof( StyleManager )] as StyleManager;
        if ( manager != null )
          return manager;

        dataContainer.Data[typeof( StyleManager )] = manager = new StyleManager( element );
        return manager;
      }

      return new StyleManager( element );
    }



    private StyleManager( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      _element = element;

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
    /// <param name="value">样式值（若为 null 则移除样式）</param>
    /// <returns>样式管理器自身</returns>
    public virtual StyleManager SetValue( string name, string value )
    {

      lock ( _element.SyncRoot )
      {

        EnsureStyle();

        _style[name] = value;

        _element.SetAttribute( "style", _style.ToString(), out _attribute );

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

        if ( _style == null || styleAttribute != _attribute || (styleAttribute == null && _style.Any()) )
        {
          _style = CssPropertyParser.ParseCssStyle( styleAttribute.Value().IfNull( "" ) );
          _attribute = styleAttribute;
        }
      }
    }


    /// <summary>
    /// 清除所有样式
    /// </summary>
    /// <returns></returns>
    public StyleManager Clear()
    {

      _style.Clear();
      _element.SetAttribute( "style", "", out _attribute );

      return this;

    }
  }
}
