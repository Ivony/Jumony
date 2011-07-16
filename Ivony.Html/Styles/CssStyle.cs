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
    public static CssStyle Style( this IHtmlElement element )
    {
      return new CssStyle( element );
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
    /// 获取元素集合的样式对象，用于方便的操纵一组元素的样式
    /// </summary>
    /// <param name="elements">要操纵样式的元素集合</param>
    /// <returns>样式对象</returns>
    private static CssStyle Style( this IEnumerable<IHtmlElement> elements )//UNDONE 暂时屏蔽
    {
      return new CssStyleSetSetter( elements );
    }
  }

}


namespace Ivony.Html.Styles
{


  /// <summary>
  /// 提供元素CSS样式管理
  /// </summary>
  public class CssStyle
  {

    private static readonly Regex styleSettingsRegex = new Regex( string.Format( CultureInfo.InvariantCulture, @"^\s*(?<styleSetting>{0})*$", Regulars.styleSettingPattern ), RegexOptions.Compiled | RegexOptions.CultureInvariant );


    private readonly Hashtable settings = Hashtable.Synchronized( new Hashtable( StringComparer.OrdinalIgnoreCase ) );

    private IHtmlElement _element;

    internal CssStyle()
    {
    }

    internal CssStyle( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      _element = element;

      lock ( element.SyncRoot )
      {
        settings = GetStyleSettings( element.Attribute( "style" ).Value() );
      }
    }


    /// <summary>
    /// 获取样式设置值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public virtual string GetValue( string name )
    {
      return (string) settings[name];
    }

    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>样式管理器自身</returns>
    public virtual CssStyle SetValue( string name, string value )
    {
      settings[name] = value;

      _element.SetAttribute( "style", GetStyleExpression( settings ) );

      return this;
    }



    /// <summary>
    /// 分析样式表达式，获取所有样式值
    /// </summary>
    /// <param name="styleExpression">样式表达式</param>
    /// <returns>样式设置值</returns>
    protected Hashtable GetStyleSettings( string styleExpression )
    {

      if ( string.IsNullOrEmpty( styleExpression ) )
        return new Hashtable();

      var match = styleSettingsRegex.Match( styleExpression );

      if ( !match.Success )
        throw new FormatException();

      foreach ( var settingCapture in match.Groups["styleSetting"].Captures.Cast<Capture>() )
      {
        string name = settingCapture.FindCaptures( match.Groups["name"] ).Single().Value;
        string value = settingCapture.FindCaptures( match.Groups["value"] ).Single().Value;

        settings.Add( name, value );
      }

      return settings;
    }



    /// <summary>
    /// 生成样式表达式
    /// </summary>
    /// <param name="styleSettings">样式设置</param>
    /// <returns>样式表达式</returns>
    protected static string GetStyleExpression( Hashtable styleSettings )
    {

      if ( styleSettings == null )
        throw new ArgumentNullException( "styleSettings" );

      var builder = new StringBuilder();

      foreach ( DictionaryEntry entry in styleSettings )
        builder.AppendFormat( "{0}: {1}; ", entry.Key, entry.Value );

      return builder.ToString().Trim();
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



  internal class CssStyleSetSetter : CssStyle
  {

    private IEnumerable<IHtmlElement> _elements;

    public CssStyleSetSetter( IEnumerable<IHtmlElement> elements )
    {
      _elements = elements;
    }

    public override string GetValue( string name )
    {
      throw new NotSupportedException( "CssStyle 对象在表示元素集的时候，不支持 Get 方法。" );
    }

    public override CssStyle SetValue( string name, string value )
    {
      _elements.ForAll( e => e.Style().SetValue( name, value ) );

      return this;
    }
  }
}
