using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public static class MvcHelpers
  {

    public static JumonyUrlHelper GetUrlHelper( RequestContext requestContext, string virtualPath )
    {
      return new JumonyUrlHelper( requestContext, virtualPath );
    }
  }
}
