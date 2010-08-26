using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Web.Html
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

    void BeginRequest( object sender, EventArgs e )
    {

      var request = HttpContext.Current.Request;

      var physicalPath = request.PhysicalPath;
      var virtualPath = request.Path;

      if ( Path.GetExtension( physicalPath ) != ".html" && Path.GetExtension( physicalPath ) != ".htm" )
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
