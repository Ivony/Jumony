using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 静态文件内容加载器，用于从静态文件中加载 HTML 内容，会自动缓存
  /// </summary>
  public class StaticFileContentProvider : IHtmlContentProvider
  {


    private static readonly ICollection<string> allowsExtensions = new ReadOnlyCollection<string>( new[] { ".html", ".htm" } );

    /// <summary>
    /// 从静态文件中加载 HTML 内容
    /// </summary>
    /// <param name="virtualPath">静态文件的虚拟路径</param>
    /// <returns>加载的内容结果</returns>
    public HtmlContentResult LoadContent( string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      return LoadContent( HostingEnvironment.VirtualPathProvider, virtualPath );
    }


    private static readonly Uri baseUri = new Uri( "virtualpath://" + Guid.NewGuid().ToString( "N" ) + "/" );


    /// <summary>
    /// 利用指定 VirtualPathProvider 将虚拟路径所指向文件当作静态文件加载。
    /// </summary>
    /// <param name="provider">指定的 VirtualPathProvider</param>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>加载结果</returns>
    public HtmlContentResult LoadContent( VirtualPathProvider provider, string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;

      if ( !provider.FileExists( virtualPath ) )
        return null;

      var file = provider.GetFile( virtualPath );

      if ( file == null )
        return null;



      var key = provider.GetCacheKey( virtualPath ) ?? "StaticFile_" + virtualPath;

      var content = HttpRuntime.Cache.Get( key ) as string;


      if ( content == null )
      {
        var dependency = HtmlServices.CreateCacheDependency( provider, virtualPath );
        content = LoadContent( file );

        HttpRuntime.Cache.Insert( key, content, dependency );
      }


      return new HtmlContentResult( content, key );
    }


    /// <summary>
    /// 从指定虚拟文件中读取文本内容
    /// </summary>
    /// <param name="file">虚拟文件</param>
    /// <returns></returns>
    public static string LoadContent( VirtualFile file )
    {
      using ( var reader = new StreamReader( file.Open(), true ) )
      {
        return reader.ReadToEnd();
      }
    }
  }
}
