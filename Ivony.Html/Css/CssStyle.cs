using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// CSS3 样式设置的实现
  /// </summary>
  public class CssStyle : CssStyleBase
  {


    private Dictionary<string, CssStyleProperty> _properties = new Dictionary<string, CssStyleProperty>();

    private object _sync = new object();

    public override object SyncRoot { get { return _sync; } }


    public CssStyle( CssStyleSpecificationBase specification )
    {
      Specification = specification;
    }


    public CssStyleSpecificationBase Specification
    {
      get;
      private set;
    }




    /// <summary>
    /// 设置样式属性
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
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
    /// 设置样式属性
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    /// <param name="important">指定是否要覆盖其他样式设置</param>
    public void SetValue( string name, string value, bool important )
    {

      if ( value.EndsWith( ";" ) )
        throw new FormatException( "value 参数值不能以 ';' 结尾。" );

      if ( value.EndsWith( importantFlag ) )
        throw new FormatException( string.Format( "value 参数值不能以 \"{0}\" 结尾。", importantFlag ) );

      var property = new CssStyleProperty( name, value, important );
      SetProperties( property );
    }


    /// <summary>
    /// 设置样式属性
    /// </summary>
    /// <param name="properties">要设置的样式属性</param>
    public void SetProperties( params CssStyleProperty[] properties )
    {
      Specification.TransformProperties( properties ).ForAll( p => SetPropertyInternal( p ) );
    }


    /// <summary>
    /// 设置样式属性
    /// </summary>
    /// <param name="property">样式属性</param>
    protected void SetPropertyInternal( CssStyleProperty property )
    {
      lock ( SyncRoot )
      {
        var name = property.Name;
        var s = GetProperty( name );
        if ( s == null || property.Important || !s.Important )
          _properties[name] = property;
      }
    }



    /// <summary>
    /// 获取样式属性值
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置值</returns>
    public string GetValue( string name )
    {
      var setting = GetProperty( name );
      if ( setting == null )
        return null;

      else
        return setting.Value;
    }


    /// <summary>
    /// 获取样式属性
    /// </summary>
    /// <param name="name">样式名</param>
    /// <returns>样式设置</returns>
    protected CssStyleProperty GetProperty( string name )
    {
      lock ( SyncRoot )
      {
        CssStyleProperty property;
        if ( _properties.TryGetValue( name, out property ) )
          return property;

        else if ( Specification.IsShorthandStyle( name ) )
          return Specification.TryGetShorthandProperty( name, this );

        else
          return null;
      }
    }
  }
}
