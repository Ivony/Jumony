using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public class JumonyRouteHandler : IRouteHandler
  {

    public static readonly string MappingKey = "JumonyRequestMapping";

    public IHttpHandler GetHttpHandler( RequestContext requestContext )
    {

      var mapping = requestContext.RouteData.DataTokens[MappingKey] as RequestMapping;
      if ( mapping == null )
        throw new InvalidOperationException();//UNDONE 详细的异常信息

      var context = requestContext.HttpContext;

      context.SetMapping( mapping );
      return WebExtenions.GetHttpHandler( mapping.Handler );
    }

    public static IRouteHandler Instance { get; set; }
  }
}
