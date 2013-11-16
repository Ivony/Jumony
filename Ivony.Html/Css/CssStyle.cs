using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 CSS 样式属性集合
  /// </summary>
  public class CssStyle : IEnumerable<CssStyleProperty>
  {


    private Dictionary<string, CssStyleProperty> _properties = new Dictionary<string, CssStyleProperty>( StringComparer.OrdinalIgnoreCase );
    private object _sync = new object();


    /// <summary>
    /// 获取用于线程同步的对象
    /// </summary>
    public object SyncRoot { get { return _sync; } }


    /// <summary>
    /// 创建 CssStyle 对象
    /// </summary>
    /// <param name="specification">要遵循的 CSS 规范</param>
    public CssStyle( CssStyleSpecificationBase specification )
    {
      Specification = specification;
    }


    /// <summary>
    /// 获取该 CssStyle 对象所遵循的 CSS 规范。
    /// </summary>
    public CssStyleSpecificationBase Specification
    {
      get;
      private set;
    }


    /// <summary>
    /// 定义 !important 标识
    /// </summary>
    public const string importantFlag = "!important";


    /// <summary>
    /// 设置样式属性
    /// </summary>
    /// <param name="name">样式名</param>
    /// <param name="value">样式值</param>
    public void SetValue( string name, string value )
    {

      if ( value == null )
      {
        RemoveProperty( name );
        return;
      }

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
    /// 移除样式属性
    /// </summary>
    /// <param name="name">要移除的样式属性名</param>
    protected void RemoveProperty( string name )
    {
      lock ( SyncRoot )
      {
        _properties.Remove( name );
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


    /// <summary>
    /// 获取或设置样式属性
    /// </summary>
    /// <param name="name">样式属性名</param>
    /// <returns>样式属性值</returns>
    public string this[string name]
    {
      get { return GetValue( name ); }
      set { SetValue( name, value ); }

    }



    /// <summary>
    /// 获取 CSS 样式的字符串表达形式
    /// </summary>
    /// <returns>CSS 样式表达式</returns>
    public override string ToString()
    {
      return string.Join( ";", _properties.Values.Select( p => p.ToString() ).ToArray() );

    }



    IEnumerator<CssStyleProperty> IEnumerable<CssStyleProperty>.GetEnumerator()
    {
      return _properties.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _properties.Values.GetEnumerator();
    }

    /// <summary>
    /// 清除所有样式
    /// </summary>
    public void Clear()
    {
      _properties.Clear();
    }
  }
}
