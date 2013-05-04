using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Ivony.Html
{

  /// <summary>
  /// 提供 HTML 编码和解码。
  /// </summary>
  public static class HtmlEncoding
  {

    /// <summary>
    /// 对字符串进行 HTML 编码。
    /// </summary>
    /// <param name="text">要编码的字符串</param>
    /// <returns>编码后的字符串</returns>
    public static string HtmlEncode( string text )
    {

#if dotnet4
      return System.Net.WebUtility.HtmlEncode( text );
#else
      using ( StringWriter writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        HtmlEncode( text, writer );

        return writer.ToString();
      }
#endif
    }

    /// <summary>
    /// 对字符串进行 HTML 编码。
    /// </summary>
    /// <param name="text">要编码的字符串</param>
    /// <param name="writer">用于写入编码后字符串的文本写入器</param>
    /// <remarks>注意！空白字符不会被自动编码</remarks>
    public static void HtmlEncode( string text, TextWriter writer )
    {
#if dotnet4
      System.Net.WebUtility.HtmlEncode( text, writer );
#else

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

        if ( ( ch >= '\x00a0' ) && ( ch < '\x0100' ) )
        {
          writer.Write( "&#" );
          writer.Write( ( (int) ch ).ToString( System.Globalization.NumberFormatInfo.InvariantInfo ) );
          writer.Write( ';' );
        }
        else
          writer.Write( ch );

      }
#endif
    }


    /// <summary>
    /// 对字符串进行 HTML 解码。
    /// </summary>
    /// <param name="htmlText">要解码的字符串</param>
    /// <returns>解码后的字符串</returns>
    public static string HtmlDecode( string htmlText )
    {
#if dotnet4
      return System.Net.WebUtility.HtmlDecode( htmlText );
#else

      using ( StringWriter writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        HtmlDecode( htmlText, writer );

        return writer.ToString();
      }
#endif

    }

    /// <summary>
    /// 对字符串进行 HTML 解码。
    /// </summary>
    /// <param name="htmlText">要解码的字符串</param>
    /// <param name="writer">用于写入解码后字符串的文本写入器</param>
    public static void HtmlDecode( string htmlText, TextWriter writer )
    {

#if dotnet4
      System.Net.WebUtility.HtmlDecode( htmlText, writer );
#else


      if ( htmlText == null )
        return;

      if ( writer == null )
        throw new ArgumentNullException( "writer" );




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

          if ( ( entityEnds > 0 ) && ( htmlText[entityEnds] == ';' ) )
          {
            string entity = htmlText.Substring( i + 1, entityEnds - i - 1 );

            if ( entity.StartsWith( "#" ) )//以#开头
            {
              ushort charCode;
              if ( ( entity[1] == 'x' ) || ( entity[1] == 'X' ) )
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
              if ( !HtmlSpecificationBase.entities.TryGetValue( entity, out ch ) )//没有找到则原样输出
              {
                writer.Write( '&' );
                writer.Write( entity );
                writer.Write( ';' );

                continue;
              }
            }
          }
        }

        writer.Write( ch );
      }

      foreach ( char ch in htmlText )
      {
        if ( ch == '&' )
        {
        }
      }
#endif
    }

    /// <summary>
    /// 将字符串最小限度地转换为 HTML 编码的字符串。
    /// </summary>
    /// <param name="text">要编码的字符串</param>
    /// <returns>编码后的字符串</returns>
    public static string HtmlAttributeEncode( string text )
    {
      using ( StringWriter writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        HtmlAttributeEncode( text, writer );

        return writer.ToString();
      }
    }

    /// <summary>
    /// 将字符串最小限度地转换为 HTML 编码的字符串。
    /// </summary>
    /// <param name="text">要编码的字符串</param>
    /// <param name="writer">用于写入编码后字符串的文本写入器</param>
    public static void HtmlAttributeEncode( string text, TextWriter writer )
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
          default:
            writer.Write( ch );
            continue;
        }

      }
    }
  }
}
