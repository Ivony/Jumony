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
  public abstract class CssStyleBase
  {
    public const string importantFlag = "!important";



    protected abstract void SetStyleSetting( CssStyleProperty setting );

    protected abstract CssStyleProperty GetStyleSetting( string name );

    protected abstract CssStyleProperty[] GetAllStyleSettings();


    public abstract object SyncRoot { get; }



    /// <summary>
    /// 获取或设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式值</returns>
    public string this[string name]
    {
      get { return GetValue( name ); }
      set { SetValue( name, value ); }
    }


    /// <summary>
    /// 获取样式设置值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public string GetValue( string name )
    {
      var setting = GetStyleSetting( name );
      if ( setting == null )
        return null;

      else
        return setting.Value;
    }



    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <returns>返回 CssStyle 对象自身，便于链式调用</returns>
    public void SetValue( string name, string value )
    {
      if ( value.EndsWith( importantFlag ) )
      {
        value = value.Remove( value.Length - importantFlag.Length );
        SetValue( name, value, true );
      }
      else
        SetValue( name, value, false );

    }


    /// <summary>
    /// 设置样式值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <param name="important">指定是否要覆盖其他样式设置</param>
    /// <returns>返回 CssStyle 对象自身，便于链式调用</returns>
    public void SetValue( string name, string value, bool important )
    {

      if ( value.EndsWith( ";" ) )
        throw new FormatException( "value 参数值不能以 ';' 结尾。" );

      if ( value.EndsWith( importantFlag ) )
        throw new FormatException( string.Format( "value 参数值不能以 \"{0}\" 结尾。", importantFlag ) );

      var setting = new CssStyleProperty( name, value, important );
      SetStyleSetting( setting );
    }


    internal void InitializeSettings( CssStyleProperty[] settings )
    {
      foreach ( var s in settings )
        SetStyleSetting( s );
    }


    /// <summary>
    /// 重写 ToString 方法输出样式设置表达式
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Join( ";", GetAllStyleSettings().Select( s => s.ToString() ).ToArray() );
    }



  }
}
