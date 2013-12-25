using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ivony.Html.Web
{

  internal class FallbackCachePolicyProvider : IMvcCachePolicyProvider
  {
    public CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      return CreateCachePolicy( context.HttpContext );
    }

    public CachePolicy CreateCachePolicy( HttpContextBase context )
    {
      return HtmlServices.GetCachePolicy( context );
    }
  }

}
