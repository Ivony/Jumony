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

  public static class CacheableResultHelper
  {
    private class CachedResponseResult : ActionResult
    {
      private ICachedResponse _cachedResponse;

      public CachedResponseResult( ICachedResponse cachedResponse )
      {
        _cachedResponse = cachedResponse;
      }

      public override void ExecuteResult( ControllerContext context )
      {
        _cachedResponse.Apply( context.HttpContext.Response );
      }
    }

    public static ActionResult GetCachedResult( this ICachableResult cachable )
    {
      return new CachedResponseResult( cachable.GetCachedResponse() );
    }


  }

}
