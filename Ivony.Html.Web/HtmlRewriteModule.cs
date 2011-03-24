using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{
  public class HtmlRewriteModule : IHttpModule
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

      var result = HtmlProviders.MapRequest( new HttpRequestWrapper( request ) );

      if ( result == null )
        return;


      var handler = result.Handler;

      var httpHandler = handler as JumonyHandler;

      if ( httpHandler == null )
        httpHandler = new HttpHandler( handler );


      context.SetMapping( result );

      context.RemapHandler( httpHandler );

    }

  }
}
