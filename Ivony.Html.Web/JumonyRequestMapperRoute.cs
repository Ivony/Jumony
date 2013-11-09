using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public class JumonyRequestMapperRoute : RouteBase
  {
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {

      var mapping = HtmlProviders.MapRequest( httpContext.Request );
      if ( mapping == null )
        return null;



      var routeData = new RouteData( this, JumonyRouteHandler.Instance );
      routeData.DataTokens[JumonyRouteHandler.MappingKey] = mapping;

      return routeData;

    }

    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {
      return null;
    }
  }
}
