using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public class HtmlHandler : IHttpHandler
  {



    public bool IsReusable
    {
      get { throw new NotImplementedException(); }
    }

    public void ProcessRequest( HttpContext context )
    {
      ProcessRequest( new HttpContextWrapper( context ) );
    }


    protected virtual void ProcessRequest( HttpContextBase context )
    {

      var routeData = context.Request.RequestContext.RouteData;

      var virtualPath = routeData.DataTokens[JumonyRequestMapperRoute.TokenKey] as string ?? context.Request.AppRelativeCurrentExecutionFilePath;



    }

  }
}
