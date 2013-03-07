using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{

  /// <summary>
  /// CSS长度值
  /// </summary>
  public sealed class CssLengthValue : CssStyleValue
  {

    public static CssLengthValue Pixels( decimal length )
    {
      return new CssLengthValue( length, "px" );
    }

    public static CssLengthValue EachM( decimal length )
    {
      return new CssLengthValue( length, "em" );
    }

    public static CssLengthValue EachX( decimal length )
    {
      return new CssLengthValue( length, "ex" );
    }

    public static CssLengthValue Inches( decimal length )
    {
      return new CssLengthValue( length, "in" );
    }

    public static CssLengthValue Centimeters( decimal length )
    {
      return new CssLengthValue( length, "cm" );
    }

    public static CssLengthValue Millimeters( decimal length )
    {
      return new CssLengthValue( length, "mm" );
    }

    public static CssLengthValue Points( decimal length )
    {
      return new CssLengthValue( length, "pt" );
    }

    public static CssLengthValue Picas( decimal length )
    {
      return new CssLengthValue( length, "pc" );
    }

    public static implicit operator CssLengthValue( decimal value )
    {
      return new CssLengthValue( value, "px" );
    }

    private CssLengthValue( decimal value, string unit )
    {
      _value = value;
      _unit = unit;
    }

    private readonly decimal _value;
    private readonly string _unit;

    public override string ValueString
    {
      get { return _value + _unit; }
    }


    public override bool Equals( object obj )
    {
      var value = obj as CssLengthValue;

      if ( value._unit != _unit )
        return false;

      if ( value._value != _value )
        return false;

      return true;
    }

    public override int GetHashCode()
    {
      return _value.GetHashCode() ^ _unit.GetHashCode();
    }

  }


  public static class CssLengthExtension
  {
    public static CssLengthValue px( this decimal value )
    {
      return CssLengthValue.Pixels( value );
    }
    public static CssLengthValue em( this decimal value )
    {
      return CssLengthValue.EachM( value );
    }
    public static CssLengthValue ex( this decimal value )
    {
      return CssLengthValue.EachX( value );
    }
    public static CssLengthValue @in( this decimal value )
    {
      return CssLengthValue.Inches( value );
    }
    public static CssLengthValue cm( this decimal value )
    {
      return CssLengthValue.Centimeters( value );
    }
    public static CssLengthValue mm( this decimal value )
    {
      return CssLengthValue.Millimeters( value );
    }
    public static CssLengthValue pt( this decimal value )
    {
      return CssLengthValue.Points( value );
    }
    public static CssLengthValue pc( this decimal value )
    {
      return CssLengthValue.Picas( value );
    }
  }

}
