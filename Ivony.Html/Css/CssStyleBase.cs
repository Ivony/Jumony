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

    /// <summary>
    /// CSS 样式 important 标识
    /// </summary>
    public const string importantFlag = "!important";


    /// <summary>
    /// 设置样式属性
    /// </summary>
    /// <param name="property">样式属性</param>
    protected abstract void SetStyleProperty( CssStyleProperty property );

    /// <summary>
    /// 获取样式属性
    /// </summary>
    /// <param name="name">样式名称</param>
    /// <returns></returns>
    protected abstract CssStyleProperty GetStyleProperty( string name );

    /// <summary>
    /// 获取所有的样式属性
    /// </summary>
    /// <returns>所有的样式属性</returns>
    protected abstract CssStyleProperty[] GetAllStyleProperties();


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
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
      var setting = GetStyleProperty( name );
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
      SetStyleProperty( setting );
    }


    internal void InitializeSettings( CssStyleProperty[] settings )
    {
      foreach ( var s in settings )
        SetStyleProperty( s );
    }


    /// <summary>
    /// 重写 ToString 方法输出样式设置表达式
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Join( ";", GetAllStyleProperties().Select( s => s.ToString() ).ToArray() );
    }



  }
}
