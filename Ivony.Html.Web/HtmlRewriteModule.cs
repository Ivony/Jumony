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
      context.BeginRequest += new EventHandler( BeginRequest );


    }



    private static string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    void BeginRequest( object sender, EventArgs e )
    {



      var request = HttpContext.Current.Request;

      var result = HtmlProviders.GetRequestData( request );

      if ( result == null )
        return;

      if ( result.Handler != null )
        throw new NotImplementedException();


      else if ( result.RewritePath != null )
      {
        HttpContext.Current.Items.Add( "HtmlRewriteModule_OriginUrl", request.Url );
        HttpContext.Current.Items.Add( "HtmlRewriteModule_ProviderResult", result );

        HttpContext.Current.RewritePath( result.RewritePath );
        return;
      }

    }

  }
}
