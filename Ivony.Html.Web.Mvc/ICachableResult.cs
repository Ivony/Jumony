using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义可被缓存的 ActionResult
  /// </summary>
  public interface ICacheableResult
  {

    /// <summary>
    /// 获取缓存的响应
    /// </summary>
    /// <returns></returns>
    ICachedResponse GetCachedResponse();

  }


  /// <summary>
  /// 提供 ICacheableResult 的一些扩展
  /// </summary>
  public static class CacheableResultHelper
  {
    private class CachedResponseResult : ActionResult
    {
      private ICachedResponse _cachedResponse;

      public CachedResponseResult( ICachedResponse cachedResponse )
      {
        if ( cachedResponse == null )
          throw new ArgumentNullException( "cachedResponse" );

        _cachedResponse = cachedResponse;
      }

      public override void ExecuteResult( ControllerContext context )
      {
        _cachedResponse.Apply( context.HttpContext.Response );
      }
    }


    /// <summary>
    /// 获取用于输出缓存的 ActionResult
    /// </summary>
    /// <param name="cachable">已被缓存的响应信息</param>
    /// <returns>用于输出缓存的 ActionResult</returns>
    public static ActionResult GetCachedResult( this ICacheableResult cachable )
    {
      var response = cachable.GetCachedResponse();
      if ( response == null )
        return null;


      return response.ToCachedResult();
    }


    /// <summary>
    /// 从缓存的输出创建一个 ActionResult
    /// </summary>
    /// <param name="response">已被缓存的输出</param>
    /// <returns>用于输出缓存的 ActionResult</returns>
    public static ActionResult ToCachedResult( this ICachedResponse response )
    {
      return new CachedResponseResult( response );
    }


  }

}
