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
      context.ResolveRequestCache += new EventHandler( OnResolveRequestCache );
      context.PostResolveRequestCache += new EventHandler( OnPreMapRequestHandler );

      _application = context;
    }

    private void OnResolveRequestCache( object sender, EventArgs e )
    {

      var context = HttpContext.Current;

      var key = HtmlProviders.GetCacheKey( new HttpContextWrapper( context ) );

      if ( key != null )
      {

        var outputCache = context.Cache.Get( key ) as string;

        if ( outputCache != null )
        {
          context.Response.Write( outputCache );

          context.Response.End();
        }

      }

    }


    private HttpApplication _application;





    private void OnPreMapRequestHandler( object sender, EventArgs e )
    {
      var context = HttpContext.Current;


      var request = context.Request;

      var result = HtmlProviders.MapRequest( request );

      if ( result == null )
        return;


      var handler = result.Handler;

      var httpHandler = handler as JumonyHandler;

      if ( httpHandler == null )
        httpHandler = new HttpHandler( handler );


      context.SetMapResult( result );

      context.RemapHandler( httpHandler );

    }

  }
}
