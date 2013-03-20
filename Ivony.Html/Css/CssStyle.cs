using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Styles;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// Css 设置容器
  /// </summary>
  public class CssStyle
  {

    private Dictionary<string, CssStyleSetting> _settings;


    /// <summary>
    /// 创建 CssSettingCollection 对象
    /// </summary>
    /// <param name="settings"></param>
    public CssStyle( CssStyleSetting[] settings )
    {
      _settings = Arrange( settings );
    }


    /// <summary>
    /// 创建 CssSettingCollection 对象
    /// </summary>
    /// <param name="styleExpression"></param>
    public CssStyle( string styleExpression )
    {
      _settings = Arrange( CssParser.ParseCssSettings( styleExpression ) );
    }


    /// <summary>
    /// 整理合并有效的样式设置
    /// </summary>
    /// <param name="settings">样式设置</param>
    /// <returns>整理合并后的样式设置</returns>
    public static Dictionary<string, CssStyleSetting> Arrange( CssStyleSetting[] settings )
    {

      var dictionary = new Dictionary<string, CssStyleSetting>();

      foreach ( var s in settings )
      {
        if ( dictionary.ContainsKey( s.Name ) )
        {
          if ( dictionary[s.Name].Important && !s.Important )
            continue;
        }

        dictionary[s.Name] = s;
      }

      return dictionary;
    }


    private object _sync = new object();

    /// <summary>
    /// 获取或设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式值</returns>
    public string this[string name]
    {
      get { lock ( _sync ) { return GetValue( name ); } }
      set { lock ( _sync ) { SetValue( name, value ); } }
    }


    /// <summary>
    /// 获取样式设置值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public virtual string GetValue( string name )
    {

      CssStyleSetting setting;
      if ( _settings.TryGetValue( name, out setting ) )
        return setting.Value;

      else
        return null;
    }


    string importantFlag = "!important";

    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>返回 CssStyle 对象自身，便于链式调用</returns>
    public virtual void SetValue( string name, string value, bool important = false )
    {

      if ( value.EndsWith( importantFlag ) )
      {
        if ( important )
          throw new FormatException( "value is invalid, cannot end with \"!important\"" );

        important = true;
        value = value.Remove( value.Length - importantFlag.Length );
      }

      var setting = new CssStyleSetting( name, value, important );
      _settings[setting.Name] = setting;
    }



    /// <summary>
    /// 生成样式表达式
    /// </summary>
    /// <param name="styleSettings">样式设置</param>
    /// <returns>样式表达式</returns>
    public static string GenerateStyleExpression( IEnumerable<CssStyleSetting> styleSettings )
    {

      if ( styleSettings == null )
        throw new ArgumentNullException( "styleSettings" );

      return string.Join( ";", styleSettings.Select( s => s.ToString() ).ToArray() );

    }

    /// <summary>
    /// 重写 ToString 方法输出样式设置表达式
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return GenerateStyleExpression( _settings.Values );
    }
  }
}
