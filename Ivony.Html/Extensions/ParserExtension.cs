using System;
using System.IO;
using System.Net;
using System.Text;

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
    /// <param name="defaultEncoding">默认编码</param>
    /// <param name="detectEncoding">是否应当检测编码，默认为true，如果检测到编码，则忽略默认编码设置，若设置为false，则强行使用默认编码。</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, string uri, Encoding defaultEncoding = null, bool detectEncoding = true )
    {

      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( uri == null )
        throw new ArgumentNullException( "uri" );


      return LoadDocument( parser, new Uri( uri ), defaultEncoding, detectEncoding );

    }

    /// <summary>
    /// 从指定的 URL 地址加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="uri">用于加载 HTML 文档的地址</param>
    /// <param name="defaultEncoding">默认编码</param>
    /// <param name="detectEncoding">是否应当检测编码，默认为true，如果检测到编码，则忽略默认编码设置，若设置为false，则强行使用默认编码。</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Uri uri, Encoding defaultEncoding = null, bool detectEncoding = true )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( uri == null )
        throw new ArgumentNullException( "uri" );

      if ( uri.IsFile )
      {
        using ( var stream = File.OpenRead( uri.LocalPath ) )
        {
          return LoadDocument( parser, stream, defaultEncoding ?? Encoding.Default, uri );
        }
      }


      var request = WebRequest.Create( uri );
      var response = request.GetResponse();

      return LoadDocument( parser, response, defaultEncoding, detectEncoding );

    }

    /// <summary>
    /// 从指定的 Web 响应数据地址加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="response">用于加载 HTML 文档的 Web 响应数据</param>
    /// <param name="detectEncoding">是否应当检测编码，默认为true，如果检测到编码，则忽略默认编码设置，若设置为false，则强行使用默认编码。</param>
    /// <param name="defaultEncoding">默认编码</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, WebResponse response, Encoding defaultEncoding = null, bool detectEncoding = true )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( response == null )
        throw new ArgumentNullException( "response" );




      if ( detectEncoding )
      {
        if ( response.Headers.HasKeys() )
        {
          var contentType = response.Headers[HttpResponseHeader.ContentType];
          if ( contentType != null )
          {
            foreach ( var value in contentType.Split( ';' ) )
            {
              var _value = value.Trim();
              if ( _value.StartsWith( "charset=", StringComparison.OrdinalIgnoreCase ) )
                defaultEncoding = Encoding.GetEncoding( _value.Substring( 8 ) );
            }
          }
        }
      }


      return LoadDocument( parser, response.GetResponseStream(), defaultEncoding ?? Encoding.Default, response.ResponseUri );
    }



    /// <summary>
    /// 从指定的流加载 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="stream">用于加载 HTML 文档的流</param>
    /// <param name="encoding">流的编码</param>
    /// <param name="uri">文档的 URL 地址</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( this IHtmlParser parser, Stream stream, Encoding encoding, Uri uri )
    {
      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      if ( stream == null )
        throw new ArgumentNullException( "stream" );

      if ( encoding == null )
        throw new ArgumentNullException( "encoding" );



      return LoadDocument( parser, new StreamReader( stream, encoding, true ), uri );
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


      var html = reader.ReadToEnd();

      var document = parser.Parse( html, uri );

      return document;

    }



    /// <summary>
    /// 从指定文本解析 HTML 文档
    /// </summary>
    /// <param name="parser">用于解析 HTML 文本的解析器</param>
    /// <param name="html">要解析的 HTML 文本</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument Parse( this IHtmlParser parser, string html )
    {

      if ( parser == null )
        throw new ArgumentNullException( "parser" );

      return parser.Parse( html, null );
    }

  }
}
