using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Collections.ObjectModel;

namespace Ivony.Html.Web
{
  public static class HtmlProviders
  {

    private static SynchronizedCollection<IHtmlProvider> _providers = new SynchronizedCollection<IHtmlProvider>();


    static HtmlProviders()
    {
      _providers.Add( new RewriteToAshxProvider() );
    }


    public static HtmlProviderResult Provide( HttpRequest request )
    {
      foreach ( var provider in _providers )
      {
        var result = provider.Provide( request );
        if ( result != null )
          return result;
      }

      return null;
    }



    public class RewriteToAshxProvider : IHtmlProvider
    {

      private static string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

      public HtmlProviderResult Provide( HttpRequest request )
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

        return new HtmlProviderResult( handlerPath, new JumonyParser() );
      }

    }


  }
}
