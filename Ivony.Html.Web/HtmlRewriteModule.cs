using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html
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

      var physicalPath = request.PhysicalPath;
      var virtualPath = request.Path;

      if ( !allowsExtensions.Contains( Path.GetExtension( physicalPath ), StringComparer.InvariantCultureIgnoreCase ) )
        return;

      if ( !File.Exists( physicalPath ) )
        return;

      var handlerPath = virtualPath + ".ashx";
      if ( !File.Exists( request.MapPath( handlerPath ) ) )
        return;

      HttpContext.Current.Items.Add( "HtmlRewriteModule_OriginUrl", request.Url );

      HttpContext.Current.RewritePath( handlerPath );
    }

  }
}
