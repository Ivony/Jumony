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
      return new StyleManager( element );
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

    private readonly CssStyle style;

    private IHtmlElement _element;

    internal StyleManager( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      _element = element;

      lock ( element.SyncRoot )
      {
        style = CssParser.ParseCssStyle( element.Attribute( "style" ).Value().IfNull( "" ) );
      }
    }


    /// <summary>
    /// 获取样式设置值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public virtual string GetValue( string name )
    {
      return style[name];
    }

    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>样式管理器自身</returns>
    public virtual StyleManager SetValue( string name, string value )
    {
      style[name] = value;

      return this;
    }





    /// <summary>
    /// 添加一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>被操作的元素</returns>
    public IHtmlElement AddClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( !classes.Contains( className ) )
          classes.Add( className );

        SetClasses( classes );
      }

      return _element;
    }


    /// <summary>
    /// 移除一个样式类
    /// </summary>
    /// <param name="className">类名</param>
    /// <returns>被操作的元素</returns>
    public IHtmlElement RemoveClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( classes.Contains( className ) )
          classes.Remove( className );

        SetClasses( classes );
      }

      return _element;
    }


    /// <summary>
    /// 获取当前应用的所有样式类
    /// </summary>
    /// <returns>样式类名集合</returns>
    public IEnumerable<string> Classes()
    {
      return GetClasses();
    }



    private void SetClasses( HashSet<string> classes )
    {
      _element.SetAttribute( "class", string.Join( " ", classes.ToArray() ) );
    }

    private HashSet<string> GetClasses()
    {
      var classSet = new HashSet<string>();


      var classes = _element.Attribute( "class" ).Value();
      if ( classes == null )
        return classSet;

      foreach ( var c in classes.Split( ' ' ) )
      {
        if ( string.IsNullOrEmpty( c ) )
          continue;

        if ( classSet.Contains( c ) )
          continue;


        classSet.Add( c );
      }

      return classSet;
    }
  }
}
