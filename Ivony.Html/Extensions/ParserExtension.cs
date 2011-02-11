using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Ivony.Html
{
  public static class ParserExtension
  {

    public static IHtmlDocument LoadDocument( this IHtmlParser parser, string url )
    {

      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( url == null )
        throw new ArgumentNullException( "url" );


      return LoadDocument( parser, new Uri( url ) );

    }

    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Uri url )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( parser == url )
        throw new ArgumentNullException( "url" );


      var request = WebRequest.Create( url );
      var response = request.GetResponse();

      return LoadDocument( parser, response );

    }

    public static IHtmlDocument LoadDocument( this IHtmlParser parser, WebResponse response )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( response == null )
        throw new ArgumentNullException( "response" );


      Encoding encoding = null;

      var contentType = response.Headers[HttpResponseHeader.ContentType];
      if ( contentType != null )
      {
        foreach ( var value in contentType.Split( ';' ) )
        {
          var _value = value.Trim();
          if ( _value.StartsWith( "charset=" ) )
            encoding = Encoding.GetEncoding( _value.Substring( 8 ) );
        }
      }


      if ( encoding != null )
        return LoadDocument( parser, new StreamReader( response.GetResponseStream(), encoding, true ), response.ResponseUri );


      return LoadDocument( parser, response.GetResponseStream(), response.ResponseUri );
    }

    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Stream stream, Uri url )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( stream == null )
        throw new ArgumentNullException( "stream" );

      if ( url == null )
        throw new ArgumentNullException( "url" );


      return LoadDocument( parser, new StreamReader( stream, true ), url );
    }

    public static IHtmlDocument LoadDocument( this IHtmlParser parser, TextReader reader, Uri url )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( reader == null )
        throw new ArgumentNullException( "reader" );

      if ( url == null )
        throw new ArgumentNullException( "url" );


      var html = reader.ReadToEnd();

      var document = parser.Parse( html, url );

      return document;

    }


  }
}
