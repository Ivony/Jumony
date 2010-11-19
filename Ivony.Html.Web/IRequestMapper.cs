using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Web.Compilation;

namespace Ivony.Html.Web
{
  public interface IRequestMapper
  {

    MapInfo MapRequest( HttpRequest request );

  }


  public class DefaultRequestMapper : IRequestMapper
  {

    private static readonly string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    public MapInfo MapRequest( HttpRequest request )
    {
      var physicalPath = request.PhysicalPath;
      var virtualPath = request.Path;

      if ( !allowsExtensions.Contains( Path.GetExtension( physicalPath ), StringComparer.InvariantCultureIgnoreCase ) )
        return null;

      if ( !File.Exists( physicalPath ) )
        return null;

      var handlerPath = virtualPath + ".ashx";
      if ( !File.Exists( request.MapPath( handlerPath ) ) )
        return null;

      return new MapInfo( new JumonyParser(), physicalPath, (IHttpHandler) BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( IHttpHandler ) ) );
    }

  }

}
