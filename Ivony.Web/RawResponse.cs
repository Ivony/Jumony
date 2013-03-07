using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace Ivony.Web
{


  /// <summary>
  /// 定义响应内容，用于缓存
  /// </summary>
  [Serializable]
  public class RawResponse : IClientCacheableResponse, ICachedResponse
  {

    /// <summary>
    /// 创建 RawResponse 对象
    /// </summary>
    public RawResponse()
    {
      Headers = new NameValueCollection();

      StatusCode = 200;
      Status = "OK";

      HeaderEncoding = Encoding.UTF8;
      ContentEncoding = Encoding.UTF8;
    }


    /// <summary>
    /// HTTP 响应状态码，如 200
    /// </summary>
    public int StatusCode
    {
      get;
      set;
    }

    /// <summary>
    /// HTTP 响应状态说明，如 OK
    /// </summary>
    public string Status
    {
      get;
      set;
    }


    /// <summary>
    /// HTTP 响应头集合
    /// </summary>
    public NameValueCollection Headers
    {
      get;
      set;
    }


    /// <summary>
    /// HTTP 响应内容
    /// </summary>
    public string Content
    {
      get;
      set;
    }

    /// <summary>
    /// 响应头的编码
    /// </summary>
    public Encoding HeaderEncoding
    {
      get;
      set;
    }

    /// <summary>
    /// 响应内容的编码
    /// </summary>
    public Encoding ContentEncoding
    {
      get;
      set;
    }


    /// <summary>
    /// 将响应直接输出
    /// </summary>
    /// <param name="response">HttpResponse 对象，用于输出响应</param>
    public virtual void Apply( HttpResponseBase response )
    {
      response.Clear();


      response.HeaderEncoding = HeaderEncoding;

      foreach ( var key in Headers.AllKeys )
      {
        foreach ( var value in Headers.GetValues( key ) )
        {
          response.AppendHeader( key, value );
        }
      }


      response.ContentEncoding = ContentEncoding;
      response.Write( Content );


    }


    /// <summary>
    /// 将响应写入响应输出流
    /// </summary>
    /// <param name="stream">输出流</param>
    public void WriteTo( Stream stream )
    {
      var headerWriter = new StreamWriter( stream, HeaderEncoding );

      headerWriter.NewLine = "\r\n";

      foreach ( var key in Headers.AllKeys )
        headerWriter.WriteLine( "{0}: {1}", key, Headers.Get( key ) );

      headerWriter.WriteLine();


      var contentWriter = new StreamWriter( stream, ContentEncoding );
      contentWriter.Write( Content );
    }


    /// <summary>
    /// 创建响应的 ETag 标识
    /// </summary>
    /// <returns>ETag 标识</returns>
    public string CreateETag()
    {

      if ( string.IsNullOrEmpty( Content ) || StatusCode != 200 )
        return null;


      return HttpServerUtility.UrlTokenEncode( CacheHelper.ComputeHash( ContentEncoding.ToString() + "\n" + Content ) );
    }
  }
}
