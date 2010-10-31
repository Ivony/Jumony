using System;
using System.Globalization;
using System.IO;

namespace Ivony.Html
{
  public class HtmlEncoding
  {
    public static string HtmlEncode( string text )
    {
      using ( StringWriter writer = new StringWriter() )
      {
        HtmlEncode( text, writer );

        return writer.ToString();
      }
    }

    public static void HtmlEncode( string text, TextWriter writer )
    {

      if ( text == null )
        return;

      if ( writer == null )
        throw new ArgumentNullException( "writer" );


      foreach ( char ch in text )
      {
        switch ( ch )
        {
          case '&':
            writer.Write( "&amp;" );
            continue;
          case '\'':
            writer.Write( "&#39;" );
            continue;
          case '"':
            writer.Write( "&quot;" );
            continue;
          case '<':
            writer.Write( "&lt;" );
            continue;
          case '>':
            writer.Write( "&gt;" );
            continue;
        }

        if ( (ch >= '\x00a0') && (ch < '\x0100') )
        {
          writer.Write( "&#" );
          writer.Write( ((int) ch).ToString( System.Globalization.NumberFormatInfo.InvariantInfo ) );
          writer.Write( ';' );
        }
        else
          writer.Write( ch );

      }
    }



    public static string HtmlDecode( string htmlText )
    {
      using ( StringWriter writer = new StringWriter() )
      {
        HtmlDecode( htmlText, writer );

        return writer.ToString();
      }
    }

    public static void HtmlDecode( string htmlText, TextWriter writer )
    {

      if ( htmlText.IndexOf( '&' ) < 0 )
      {
        writer.Write( htmlText );
        return;
      }
      int length = htmlText.Length;
      for ( int i = 0; i < length; i++ )
      {
        char ch = htmlText[i];
        if ( ch == '&' )
        {
          int entityEnds = htmlText.IndexOfAny( new[] { ';', '&' }, i + 1 );

          if ( (entityEnds > 0) && (htmlText[entityEnds] == ';') )
          {
            string entity = htmlText.Substring( i + 1, entityEnds - i - 1 );

            if ( entity.StartsWith( "#" ) )//以#开头
            {
              ushort charCode;
              if ( (entity[1] == 'x') || (entity[1] == 'X') )
                ushort.TryParse( entity.Substring( 2 ), NumberStyles.AllowHexSpecifier, (IFormatProvider) NumberFormatInfo.InvariantInfo, out charCode );
              else
                ushort.TryParse( entity.Substring( 1 ), NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out charCode );



              if ( charCode != 0 )
              {
                ch = (char) charCode;
                i = entityEnds;
              }
            }
            else
            {
              i = entityEnds;
              if ( !HtmlSpecification.entities.TryGetValue( entity, out ch ) )//没有找到则原样输出
              {
                writer.Write( '&' );
                writer.Write( entity );
                writer.Write( ';' );

                continue;
              }
            }
          }
          writer.Write( ch );
        }
      }

      foreach ( char ch in htmlText )
      {
        if ( ch == '&' )
        {
        }
      }
    }


    internal static string HtmlAttributeEncode( string text )
    {
      using ( StringWriter writer = new StringWriter() )
      {
        HtmlAttributeEncode( text, writer );

        return writer.ToString();
      }
    }

    public static void HtmlAttributeEncode( string text, TextWriter writer )
    {
      foreach ( char ch in text )
      {
        switch ( ch )
        {
          case '&':
            writer.Write( "&amp;" );
            continue;
          case '\'':
            writer.Write( "&#39;" );
            continue;
          case '"':
            writer.Write( "&quot;" );
            continue;
          case '<':
            writer.Write( "&lt;" );
            continue;
          default:
            writer.Write( ch );
            continue;
        }

      }
    }
  }
}
