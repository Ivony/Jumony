using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  public class CssBox<T> where T : CssStyleValue
  {

    public CssBox( T value ) : this( value, value ) { }
    public CssBox( T top, T left ) : this( top, left, top ) { }
    public CssBox( T top, T left, T bottom ) : this( top, left, bottom, left ) { }

    public CssBox( T top, T left, T bottom, T right )
    {
      Top = top;
      Left = left;
      Bottom = bottom;
      Right = right;
    }

    public string GetShortExpression( string name )
    {

      if ( Left.Equals( Right ) )
      {
        if ( Top.Equals( Bottom ) )
        {

          if ( Top.Equals( Left ) )
            return string.Format( "{0}: {1}", name, Top );


          return string.Format( "{0}: {1} {2}", name, Top, Left );

        }

        return string.Format( "{0}: {1} {2} {3}", name, Top, Left, Bottom );
      }

      return string.Format( "{0}: {1} {2} {3} {4}", name, Top, Left, Bottom, Right );
    }

    public string[] GetFullExpression( string formatTemplate )
    {
      string[] settings = new string[4];
      settings[0] = string.Format( "{0}: {1}", string.Format( formatTemplate, "top" ), Top );
      settings[1] = string.Format( "{0}: {1}", string.Format( formatTemplate, "left" ), Left );
      settings[2] = string.Format( "{0}: {1}", string.Format( formatTemplate, "bottom" ), Bottom );
      settings[3] = string.Format( "{0}: {1}", string.Format( formatTemplate, "right" ), Right );

      return settings;
    }


    public T Top { get; set; }
    public T Left { get; set; }
    public T Bottom { get; set; }
    public T Right { get; set; }


    public static implicit operator CssBox<T>( T value )
    {
      return new CssBox<T>( value );
    }

    public static implicit operator CssBox<T>( T[] value )
    {
      switch ( value.Length )
      {
        case 1:
          return new CssBox<T>( value[0] );
        case 2:
          return new CssBox<T>( value[0], value[1] );
        case 3:
          return new CssBox<T>( value[0], value[1], value[2] );
        case 4:
          return new CssBox<T>( value[0], value[1], value[2], value[3] );
        default:
          throw new InvalidCastException();
      }
    }


  }
}