using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 定义可被缓存的 ActionResult
  /// </summary>
  public interface ICachableResult
  {

    ICachedResponse GetCachedResponse();

  }


  /// <summary>
  /// 提供 ICachableResult 的一些扩展
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
    public static ActionResult GetCachedResult( this ICachableResult cachable )
    {
      var cache = cachable.GetCachedResponse();
      if ( cache == null )
        return null;


      return new CachedResponseResult( cache );
    }


  }

}
