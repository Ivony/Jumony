using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Web.Caching;

namespace Ivony.Html.Web
{

  public interface IHtmlContentProvider
  {

    string LoadContent( HttpContextBase context, string virtualPath );

  }


  /// <summary>
  /// 静态文件内容加载器，用于从静态文件中加载HTML内容，会自动缓存
  /// </summary>
  public class StaticFileLoader : IHtmlContentProvider
  {

    protected static Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }


    private static readonly string[] allowsExtensions = new[] { ".html", ".htm" };

    public string LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;

      return LoadContent( HostingEnvironment.VirtualPathProvider, virtualPath );
    }


    public static string LoadContent( VirtualPathProvider provider, string virtualPath )
    {

      if ( provider.FileExists( virtualPath ) )
        return null;

      var file = provider.GetFile( virtualPath );

      if ( file == null )
        return null;


      var key = string.Format( "StaticFileLoader_VirtualPath_{0}_{1}", provider.ToString(), virtualPath );

      var fileInfo = Cache.Get( key ) as VirtualFileInfo;



      if ( fileInfo == null || fileInfo.VirtualPath != virtualPath )
      {
        fileInfo = new VirtualFileInfo() { VirtualPath = virtualPath, Content = LoadContent( file ) };

        var dependency = provider.GetCacheDependency( virtualPath, null, DateTime.UtcNow );

        Cache.Insert( key, fileInfo, dependency );
      }



      return fileInfo.Content;
    }

    protected static string LoadContent( VirtualFile file )
    {
      using ( var reader = new StreamReader( file.Open(), true ) )
      {
        return reader.ReadToEnd();
      }
    }

    protected class VirtualFileInfo
    {
      public string VirtualPath
      {
        get;
        set;
      }

      public string Content
      {
        get;
        set;
      }
    }

  }


  public class AspxFileLoader : IHtmlContentProvider
  {

    private static readonly string[] allowsExtensions = new[] { ".aspx" };

    public string LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      using ( var writer = new StringWriter() )
      {
        context.Server.Execute( virtualPath, writer, false );

        return writer.ToString();
      }
    }
  }

}
