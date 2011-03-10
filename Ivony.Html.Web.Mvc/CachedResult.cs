using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public class CachedResult : ActionResult
  {

    private ICachedResponse _cachedResponse;

    public CachedResult( ICachedResponse cachedResponse )
    {
      _cachedResponse = cachedResponse;
    }

    public override void ExecuteResult( ControllerContext context )
    {
      _cachedResponse.Apply( context.HttpContext.Response );
    }
  }

}
