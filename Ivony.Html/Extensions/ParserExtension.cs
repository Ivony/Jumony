using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Ivony.Html
{
  
  
  /// <summary>
  /// 提供 HTML 解析器的一些扩展
  /// </summary>
  public static class ParserExtension
  {

    /// <summary>
    /// 从指定的 URL 地址加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="uri">用于加载 HTML 文档的地址</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, string uri )
    {

      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( uri == null )
        throw new ArgumentNullException( "uri" );


      return LoadDocument( parser, new Uri( uri ) );

    }

    /// <summary>
    /// 从指定的 URL 地址加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="uri">用于加载 HTML 文档的地址</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Uri uri )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( uri == null )
        throw new ArgumentNullException( "uri" );


      var request = WebRequest.Create( uri );
      var response = request.GetResponse();

      return LoadDocument( parser, response );

    }

    /// <summary>
    /// 从指定的 Web 响应数据地址加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="response">用于加载 HTML 文档的 Web 响应数据</param>
    /// <returns>HTML 文档对象</returns>
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



    /// <summary>
    /// 从指定的流加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="stream">用于加载 HTML 文档的流</param>
    /// <param name="uri">文档的 URL 地址</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Stream stream, Uri uri )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( stream == null )
        throw new ArgumentNullException( "stream" );

      if ( uri == null )
        throw new ArgumentNullException( "uri" );


      return LoadDocument( parser, new StreamReader( stream, true ), uri );
    }

    /// <summary>
    /// 从指定的文本读取器加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="reader">用于加载 HTML 文档的文本读取器</param>
    /// <param name="uri">文档的 URL 地址</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, TextReader reader, Uri uri )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( reader == null )
        throw new ArgumentNullException( "reader" );

      if ( uri == null )
        throw new ArgumentNullException( "url" );


      var html = reader.ReadToEnd();

      var document = parser.Parse( html, uri );

      return document;

    }


  }
}
