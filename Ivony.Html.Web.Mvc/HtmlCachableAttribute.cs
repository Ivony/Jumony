using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
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
        UpdateCache( cache );

      base.OnActionExecuted( filterContext );
    }

    protected void UpdateCache( ActionResult cache )
    {
    }

    protected ActionResult GetCachedResult( ActionResult result )
    {

      var cache = result as ICachableResult;
      if ( cache != null )
        return cache.CachedResult;

      var view = result as ViewResult;
      cache = view.View as ICachableResult;
      if ( cache != null )
        return cache.CachedResult;

      var content = result as ContentResult;
      if ( content != null )
        return content;



      return null;

    }
  }
}
