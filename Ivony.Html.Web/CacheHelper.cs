using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Security.Cryptography;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 提供 HTTP 客户端缓存的一些帮助功能
  /// </summary>
  public static class CacheHelper
  {


    /// <summary>
    /// 检查客户端的 ETag 是否已经过期，若未过期则发出 HTTP 304
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="etag">用于与客户端 ETag 比较的，生成的请求内容的 ETag</param>
    /// <returns>是否已经发出 HTTP 304</returns>
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
    /// 输出 304 通知浏览器此页未被修改
    /// </summary>
    /// <param name="context"></param>
    public static void NotModified( HttpContextBase context )
    {
      context.Response.StatusCode = 304;
      context.Response.StatusDescription = "Not Modified";
      context.Response.Cache.SetCacheability( HttpCacheability.Public );
      context.Response.End();
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
