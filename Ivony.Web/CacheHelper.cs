using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Security.Cryptography;
using System.Web.Routing;

namespace Ivony.Web
{
  /// <summary>
  /// 提供 HTTP 客户端缓存的一些帮助功能
  /// </summary>
  public static class CacheHelper
  {


    /// <summary>
    /// 检查客户端的 ETag 是否已经过期，若未过期则直接发出一个 HTTP 304 响应
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="etag">用于与客户端 ETag 比较的，生成的请求内容的 ETag</param>
    /// <returns>ETag 是否未过期</returns>
    public static bool IsNotModified( HttpContextBase context, string etag )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( etag == null )
        throw new ArgumentNullException( "etag" );


      var request = context.Request;
      var requestETag = request.Headers["If-None-Match"];


      if ( string.Equals( requestETag, etag ) )
      {
        NotModified( context );
        return true;

      }

      else
        return false;
    }


    /// <summary>
    /// 检查客户端的 ETag 是否已经过期，若未过期则产生一个 HTTP 304 响应
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="etag">用于与客户端 ETag 比较的，生成的请求内容的 ETag</param>
    /// <param name="response">需要发出的 304 响应信息</param>
    /// <returns>ETag 是否未过期</returns>
    public static bool IsNotModified( HttpContextBase context, string etag, out RawResponse response )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( etag == null )
        throw new ArgumentNullException( "etag" );


      var request = context.Request;
      var requestETag = request.Headers["If-None-Match"];


      if ( string.Equals( requestETag, etag ) )
      {
        response = NotModified();
        return true;

      }

      else
      {
        response = null;
        return false;
      }

    }


    /// <summary>
    /// 发送一个 304 通知浏览器此页未被修改
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    public static void NotModified( HttpContextBase context )
    {
      var response = NotModified();
      response.Apply( context.Response );
    }

    /// <summary>
    /// 产生一个  HTTP 304 响应通知浏览器此页未被修改
    /// </summary>
    public static RawResponse NotModified()
    {
      return new RawResponse()
      {
        StatusCode = 304,
        Status = "Not Modified"
      };
    }

    /// <summary>
    /// 从文件路径创建 ETag
    /// </summary>
    /// <param name="staticFilepath"></param>
    /// <returns></returns>
    public static string CreateETagFromFile( string staticFilepath )
    {

      var physicalPath = MapPath( staticFilepath );
      var modified = File.GetLastWriteTimeUtc( physicalPath );

      return HttpServerUtility.UrlTokenEncode( ComputeHash( physicalPath + modified.ToString( "O" ) ) );
    }








    /// <summary>
    /// 计算字符串的哈希值
    /// </summary>
    /// <param name="data">字符串数据</param>
    /// <returns></returns>
    public static byte[] ComputeHash( string data )
    {
      using ( var hashProvider = new SHA256Managed() )
      {
        return hashProvider.ComputeHash( Encoding.UTF8.GetBytes( data ) );
      }
    }



    /// <summary>
    /// 映射物理路径
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    public static string MapPath( string virtualPath )
    {
      return HostingEnvironment.MapPath( virtualPath );
    }



  }
}
