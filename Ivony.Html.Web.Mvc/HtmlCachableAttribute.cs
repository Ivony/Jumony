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

    public override void OnResultExecuted( ResultExecutedContext filterContext )
    {

      var result = filterContext.Result;

      var cache = GetCachedResult( result );

      base.OnResultExecuted( filterContext );
    }

    private object GetCachedResult( ActionResult result )
    {
      throw new NotImplementedException();
    }
  }
}
