using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;

namespace Ivony.Html.Web
{
  public interface IHtmlProvider
  {

    RequestData GetRequestData( HttpRequest request );


  }


  public class RewriteToAshxProvider : IHtmlProvider
  {

    private static string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    public RequestData GetRequestData( HttpRequest request )
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

      return new RequestData( handlerPath, new JumonyParser(), physicalPath );
    }

  }

}
