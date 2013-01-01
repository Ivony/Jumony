using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Web.Caching;
using System.Collections.ObjectModel;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 HTML 内容提供程序
  /// </summary>
  public interface IHtmlContentProvider
  {

    /// <summary>
    /// 加载 HTML 内容
    /// </summary>
    /// <param name="virtualPath">要加载内容的虚拟路径</param>
    /// <returns>加载的 HTML 内容</returns>
    HtmlContentResult LoadContent( string virtualPath );

  }


  /// <summary>
  /// IHtmlContentProvider 的内容加载结果
  /// </summary>
  public class HtmlContentResult
  {


    /// <summary>
    /// 创建 HtmlContentResult 实例
    /// </summary>
    /// <param name="provider">负责加载的提供程序</param>
    /// <param name="content">加载的内容</param>
    /// <param name="virtualPath">内容的虚拟路径</param>
    public HtmlContentResult( IHtmlContentProvider provider, string content, string virtualPath ) : this( provider, content, virtualPath, null ) { }

    /// <summary>
    /// 创建 HtmlContentResult 实例
    /// </summary>
    /// <param name="provider">负责加载的提供程序</param>
    /// <param name="content">加载的内容</param>
    /// <param name="virtualPath">内容的虚拟路径</param>
    /// <param name="cacheKey">缓存内容所使用的缓存键</param>
    public HtmlContentResult( IHtmlContentProvider provider, string content, string virtualPath, string cacheKey )
    {

      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      if ( content == null )
        throw new ArgumentNullException( "content" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlProviders.VirtualPathFormatError( "virtualPath" );

      Provider = provider;
      Content = content;
      VirtualPath = virtualPath;
      CacheKey = cacheKey;
    }


    /// <summary>
    /// HTML 文本内容
    /// </summary>
    public string Content
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取产生此结果的 HtmlContentProvider
    /// </summary>
    public IHtmlContentProvider Provider
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取缓存时使用的索引键
    /// </summary>
    public string CacheKey
    {
      get;
      private set;
    }

    /// <summary>
    /// 加载内容的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }

  }


  /// <summary>
  /// 静态文件内容加载器，用于从静态文件中加载 HTML 内容，会自动缓存
  /// </summary>
  public class StaticFileLoader : IHtmlContentProvider
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

        var now = DateTime.UtcNow;

        content = LoadContent( file );
        var dependency = provider.GetCacheDependency( virtualPath, new[] { virtualPath }, now ) ?? new CacheDependency( HostingEnvironment.MapPath( virtualPath ) );


        HttpRuntime.Cache.Insert( key, content, dependency );
      }


      return new HtmlContentResult( this, content, virtualPath, key );
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

  /// <summary>
  /// ASPX 文件内容加载器，用于从 ASPX 动态页面中加载内容
  /// </summary>
  public class AspxFileLoader : IHtmlContentProvider
  {

    private static readonly string[] allowsExtensions = new[] { ".aspx" };

    /// <summary>
    /// 读取 ASPX 页面所呈现的 HTML 内容
    /// </summary>
    /// <param name="virtualPath">ASPX 文件路径</param>
    /// <returns>ASPX 页面所呈现的 HTML 内容</returns>
    public HtmlContentResult LoadContent( string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;



      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      using ( var writer = new StringWriter() )
      {
        HttpContext.Current.Server.Execute( virtualPath, writer, false );

        return new HtmlContentResult( this, writer.ToString(), virtualPath );
      }
    }
  }

}
