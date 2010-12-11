using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{
  public class DefaultRequestMapper : IRequestMapper
  {

    private static readonly string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    public MapInfo MapRequest( HttpRequest request )
    {
      var virtualPath = request.FilePath;

      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ), StringComparer.InvariantCultureIgnoreCase ) )
        return null;

      if ( !FileExists( virtualPath ) )
        return null;

      var handlerPath = virtualPath + ".ashx";
      if ( !FileExists( handlerPath ) )
        return null;

      var handler = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( JumonyHandler ) ) as JumonyHandler;
      if ( handler == null )
        return null;

      return new MapInfo( virtualPath, handler );
    }


    private static bool FileExists( string virtualPath )
    {
      return HostingEnvironment.VirtualPathProvider.FileExists( virtualPath );
    }

  }
}
