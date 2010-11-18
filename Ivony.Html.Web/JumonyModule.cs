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
      context.BeginRequest += new EventHandler( BeginRequest );


    }



    private static string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    void BeginRequest( object sender, EventArgs e )
    {



      var request = HttpContext.Current.Request;

      var result = RequestMappers.MapRequest( request );

      if ( result == null )
        return;


      result.OriginUrl = request.Url;

      HttpContext.Current.SetMapResult( result );




      if ( result.Handler != null )
        throw new NotImplementedException();


      else if ( result.RewritePath != null )
      {

        HttpContext.Current.SetOriginUrl();
        HttpContext.Current.RewritePath( result.RewritePath );

        return;
      }

    }

  }
}
