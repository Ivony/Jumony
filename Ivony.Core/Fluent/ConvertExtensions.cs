using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  public static class ConvertExtensions
  {

    public static T CastTo<T>( this object obj )
    {
      return (T) obj;
    }

    public static T ConvertTo<T>( this object value )
    {
      if ( Convertor<T>.castMethod != null )
        return Convertor<T>.castMethod( value );


      return (T) value;
    }

    static ConvertExtensions()
    {
      Convertor<short>.castMethod    = Convert.ToInt16;
      Convertor<int>.castMethod      = Convert.ToInt32;
      Convertor<long>.castMethod     = Convert.ToInt64;
      Convertor<byte>.castMethod     = Convert.ToByte;
      Convertor<ushort>.castMethod   = Convert.ToUInt16;
      Convertor<uint>.castMethod     = Convert.ToUInt32;
      Convertor<ulong>.castMethod    = Convert.ToUInt64;
      Convertor<sbyte>.castMethod    = Convert.ToSByte;
      Convertor<float>.castMethod    = Convert.ToSingle;
      Convertor<double>.castMethod   = Convert.ToDouble;
      Convertor<decimal>.castMethod  = Convert.ToDecimal;
      Convertor<bool>.castMethod     = Convert.ToBoolean;
      Convertor<DateTime>.castMethod = Convert.ToDateTime;
      Convertor<string>.castMethod   = Convert.ToString;
    }

    private class Convertor<T>
    {
      public static Func<object, T> castMethod;
    }

    public static T IfNull<T>( this T value, T defaultValue )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return defaultValue;
      else
        return value;
    }


  }
}
