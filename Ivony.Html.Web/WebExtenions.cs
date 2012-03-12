using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供一些 Web 相关的扩展方法。
  /// </summary>
  public static class WebExtenions
  {


    private const string requestDataToken = "Jumony_HttpContext_RequestMapping";

    /// <summary>
    /// 获取请求的映射信息
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <returns>映射信息</returns>
    public static RequestMapping GetMapping( this HttpContext context )
    {
      return GetMapping( new HttpContextWrapper( context ) );
    }

    /// <summary>
    /// 获取请求的映射信息
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <returns>映射信息</returns>
    public static RequestMapping GetMapping( this HttpContextBase context )
    {
      return (RequestMapping) context.Items[requestDataToken];
    }

    internal static void SetMapping( this HttpContext context, RequestMapping data )
    {
      context.Items[requestDataToken] = data;
    }

    internal static void SetMapping( this HttpContextBase context, RequestMapping data )
    {
      context.Items[requestDataToken] = data;
    }


    /// <summary>
    /// 获取当前 HTTP 请求的客户端缓存策略
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ClientCachePolicyBase GetClientCachePolicy( this HttpContextBase context )
    {
      var instance = context.Items[ClientCachePolicy.Token] as ClientCachePolicy;

      if ( instance == null )
        return new ClientCachePolicyWrapper( context.Response.Cache );

      else
        return instance;
    }

  }
}
