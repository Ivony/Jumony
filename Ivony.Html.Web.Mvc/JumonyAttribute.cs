using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyAttribute : FilterAttribute, IActionFilter
  {

    public void OnActionExecuted( ActionExecutedContext filterContext )
    {
      var viewResult = filterContext.Result as ViewResultBase;

      if ( viewResult != null )
        filterContext.Result = new ViewResultWrapper( viewResult );
    }

    public void OnActionExecuting( ActionExecutingContext filterContext )
    {
    }


    private class ViewResultWrapper : ActionResult
    {

      private ViewResultBase _viewResult;

      public ViewResultWrapper( ViewResultBase viewResult )
      {
        _viewResult = viewResult;
      }


      public override void ExecuteResult( ControllerContext context )
      {

      }

    }

  }
}
