using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
{
  public static class HttpContextExtensions
  {
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
