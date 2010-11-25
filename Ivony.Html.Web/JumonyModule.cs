using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{
  public class JumonyModule : IHttpModule
  {


    public void Dispose()
    {
    }

    public void Init( HttpApplication context )
    {
      context.PostResolveRequestCache += new EventHandler( OnPreMapRequestHandler );
    }

    void OnPreMapRequestHandler( object sender, EventArgs e )
    {
      var context = HttpContext.Current;


      var request = context.Request;

      var result = HtmlProviders.MapRequest( request );

      if ( result == null )
        return;


//      result.OriginUrl = request.Url;

      context.SetMapResult( result );

      context.RemapHandler( result.Handler );

    }

  }
}
