using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Ivony.Html.Web.Mvc
{
  /*
  /// <summary>
  /// 用于指定 Action 或 Controller 应缓存输出结果。
  /// </summary>
  public class HtmlCachableAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {

    }

    public override void OnActionExecuted( ActionExecutedContext filterContext )
    {

      var result = filterContext.Result;

      var cache = GetCachedResult( result );

      if ( cache != null )
        UpdateCache( cache, filterContext.HttpContext );

      base.OnActionExecuted( filterContext );
    }

    protected IHtmlCachePolicyProvider CachePolicyProvider
    {
      get;
      set;
    }


    protected void UpdateCache( ActionResult cache, HttpContextBase httpContext )
    {

      throw new NotImplementedException();

      string cacheKey;
      HtmlCachePolicy cachePolicy;

      if ( CachePolicyProvider != null )
      {
        cacheKey = CachePolicyProvider.GetCacheKey( httpContext );
      }
      else
      {
        cacheKey = HtmlProviders.GetCacheKey( httpContext );
      }

      




    }

    protected ActionResult GetCachedResult( ActionResult result )
    {

      var cache = result as ICachableResult;
      if ( cache != null )
        return cache.GetCachedResult();

      var view = result as ViewResult;
      cache = view.View as ICachableResult;
      if ( cache != null )
        return cache.GetCachedResult();

      var content = result as ContentResult;
      if ( content != null )
        return content;



      return null;

    }
  }

  */
}
