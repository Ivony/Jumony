using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  /// <summary>
  /// 提供类型转换扩展方法
  /// </summary>
  public static class ConvertExtensions
  {

    /// <summary>
    /// 将对象强制类型转换为 T 类型。
    /// </summary>
    /// <typeparam name="T">要转换的目标类型</typeparam>
    /// <param name="obj">要转换的对象</param>
    /// <returns>转换后的结果</returns>
    public static T CastTo<T>( this object obj )
    {
      return (T) obj;
    }

    /// <summary>
    /// 尝试将对象转换为指定的类型
    /// </summary>
    /// <typeparam name="T">要转换的目标类型</typeparam>
    /// <param name="value">要转换的对象</param>
    /// <returns>转换后的结果</returns>
    public static T ConvertTo<T>( this object value )
    {
      if ( Convertor<T>.castMethod != null )
        return Convertor<T>.castMethod( value );


      return (T) value;
    }

    static ConvertExtensions()
    {
      Convertor<short>.castMethod = Convert.ToInt16;
      Convertor<int>.castMethod = Convert.ToInt32;
      Convertor<long>.castMethod = Convert.ToInt64;
      Convertor<byte>.castMethod = Convert.ToByte;
      Convertor<ushort>.castMethod = Convert.ToUInt16;
      Convertor<uint>.castMethod = Convert.ToUInt32;
      Convertor<ulong>.castMethod = Convert.ToUInt64;
      Convertor<sbyte>.castMethod = Convert.ToSByte;
      Convertor<float>.castMethod = Convert.ToSingle;
      Convertor<double>.castMethod = Convert.ToDouble;
      Convertor<decimal>.castMethod = Convert.ToDecimal;
      Convertor<bool>.castMethod = Convert.ToBoolean;
      Convertor<DateTime>.castMethod = Convert.ToDateTime;
      Convertor<string>.castMethod = Convert.ToString;
    }

    private class Convertor<T>
    {
      public static Func<object, T> castMethod;
    }

    /// <summary>
    /// 若值是 null 则使用指定值代换
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="value">原始值</param>
    /// <param name="defaultValue">当值为 null 时用于代换的默认值</param>
    /// <returns>原始值，当原始值不为 null 或 DBbNull，否则使用代换的默认值</returns>
    public static T IfNull<T>( this T value, T defaultValue )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return defaultValue;
      else
        return value;
    }

    /// <summary>
    /// 若值是 null 则使用指定值代换
    /// </summary>
    /// <param name="value">原始值</param>
    /// <param name="defaultValue">当值为 null 时用于代换的默认值</param>
    /// <returns>当原始值不为 null 或 DBbNull，返回原始值，否则返回代换的默认值</returns>
    public static object IfNull( this object value, object defaultValue )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return defaultValue;
      else
        return value;
    }


    /// <summary>
    /// 若值是 null 则使用指定值代换
    /// </summary>
    /// <typeparam name="TInput">值类型</typeparam>
    /// <typeparam name="TOut">输出值类型</typeparam>
    /// <param name="value">原始值</param>
    /// <param name="defaultValue">当值为 null 时用于代换的默认值</param>
    /// <param name="converter">当值不为空时，对值进行类型转换的转换器。</param>
    /// <returns>当原始值不为 null 或 DBbNull，返回转换后的原始值，否则返回代换的默认值</returns>
    public static TOut IfNull<TInput, TOut>( this TInput value, TOut defaultValue, Func<TInput, TOut> converter )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return defaultValue;

      else
      {
        if ( converter == null )
          throw new ArgumentNullException( "converter" );

        return converter( value );
      }
    }



    /// <summary>
    /// 将对象所有属性转换为对象图
    /// </summary>
    /// <param name="obj">要转换为对象图的对象</param>
    /// <returns>对象图</returns>
    public static IDictionary<string, string> ToPropertiesMap( this object obj )
    {
      if ( obj == null )
        return null;
      
      var dictionary = new Dictionary<string, string>();

      foreach ( PropertyDescriptor property in TypeDescriptor.GetProperties( obj ) )
      {
        var key = property.Name;
        var _value = property.GetValue( obj );

        string value = null;

        if ( _value != null )
          value = _value.ToString();

        dictionary.Add( key, value );
      }

      return dictionary;
    }


  }
}
