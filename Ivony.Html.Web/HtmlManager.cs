using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public class HtmlManager
  {

    protected virtual VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

    protected virtual Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }



    public virtual IHtmlDocument GetDocument( string virtualPath )
    {

      string key = CreateCacheKey( virtualPath );

      string htmlContent = Cache.Get( key ) as string;

      if ( htmlContent == null )
      {
        htmlContent = LoadFile( virtualPath );

        var dependency = VirtualPathProvider.GetCacheDependency( virtualPath, null, DateTime.UtcNow );

        Cache.Insert( key, htmlContent, dependency );
      }

      return ParseDocument( virtualPath, htmlContent );

    }


    protected IHtmlParser GetParser( string virtualPath, string htmlContent )
    {
      return new JumonyParser();
    }

    private string CreateCacheKey( string virtualPath )
    {
      return "Jumony_HtmlManager_DocumentCache_" + virtualPath;
    }

    protected virtual string LoadFile( string virtualPath )
    {
      var file = VirtualPathProvider.GetFile( virtualPath );

      using ( var stream = file.Open() )
      {
        return new StreamReader( stream ).ReadToEnd();
      }
    }

    protected virtual IHtmlDocument ParseDocument( string virtualPath, string htmlContent )
    {
      var parser = new JumonyParser();

      return parser.Parse( htmlContent );
    }


  }


  internal static class Extensions
  {
    public static IHtmlDocument MakeCopy( this IHtmlParser parser, IHtmlDocument document )
    {
      var copy = parser.Parse( null );
      var factory = copy.GetNodeFactory();
      var fragment = factory.MakeFragment( document );
      fragment.InsertTo( copy, 0 );

      return copy;
    }

  }



}
