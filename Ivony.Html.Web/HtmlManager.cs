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
  public static class HtmlManager
  {

    private static VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

    private static Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }






    private static string CreateCacheKey( string virtualPath )
    {
      return "Jumony_HtmlManager_DocumentCache_" + virtualPath;
    }




    public static IHtmlParser GetParser( string htmlContent, VirtualFile file )
    {
      return new JumonyParser();
    }




    public static IHtmlDocument LoadDocument( string virtualPath )
    {

      string key = CreateCacheKey( virtualPath );

      string htmlContent = Cache.Get( key ) as string;

      var file = VirtualPathProvider.GetFile( virtualPath );

      if ( htmlContent == null )
      {
        htmlContent = LoadFile( file );

        var dependency = VirtualPathProvider.GetCacheDependency( virtualPath, null, DateTime.UtcNow );

        Cache.Insert( key, htmlContent, dependency );
      }


      return ParseDocument( htmlContent, file );

    }

    private static string LoadFile( VirtualFile file )
    {
      if ( file == null )
        throw new ArgumentNullException( "file" );

      using ( var stream = file.Open() )
      {
        using ( var reader = new StreamReader( stream ) )
        {
          return reader.ReadToEnd();
        }
      }
    }


    public static IHtmlDocument LoadDocument( VirtualFile file )
    {
      if ( file == null )
        throw new ArgumentNullException( "file" );


      var content = LoadFile( file );

      return ParseDocument( content, file );
    }

    private static IHtmlDocument ParseDocument( string content, VirtualFile file )
    {
      var parser = GetParser( content, file );

      return parser.Parse( content );
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
