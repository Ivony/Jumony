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

    HtmlContentResult LoadContent( HttpContextBase context, string virtualPath );

  }

  public class HtmlContentResult
  {



    public HtmlContentResult( IHtmlContentProvider provider, string content, Uri contentUri ) : this( provider, content, contentUri, null ) { }

    public HtmlContentResult( IHtmlContentProvider provider, string content, Uri contentUri, string cacheKey )
    {

      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      if ( content == null )
        throw new ArgumentNullException( "content" );

      if ( contentUri == null )
        throw new ArgumentNullException( "contentUri" );

      if ( !contentUri.IsAbsoluteUri )
        throw new ArgumentException( "contentUri必须是一个绝对地址", "contentUri" );

      Provider = provider;
      Content = content;
      Url = contentUri;
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
    /// 获取或设置缓存时使用的索引键
    /// </summary>
    public string CacheKey
    {
      get;
      private set;
    }

    /// <summary>
    /// 加载的内容的URL
    /// </summary>
    public Uri Url
    {
      get;
      private set;
    }
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

    public HtmlContentResult LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      virtualPath = VirtualPathUtility.ToAppRelative( virtualPath );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;


      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      return LoadContent( HostingEnvironment.VirtualPathProvider, virtualPath );
    }

    /// <summary>
    /// 利用指定VirtualPathProvider将虚拟路径所指向文件当作静态文件加载。
    /// </summary>
    /// <param name="provider">指定的VirtualPathProvider</param>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>加载结果</returns>
    public HtmlContentResult LoadContent( VirtualPathProvider provider, string virtualPath )
    {

      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      virtualPath = VirtualPathUtility.ToAppRelative( virtualPath );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;



      if ( !provider.FileExists( virtualPath ) )
        return null;

      var file = provider.GetFile( virtualPath );

      if ( file == null )
        return null;



      var key = provider.GetCacheKey( virtualPath ) ?? "StaticFile_" + virtualPath;

      var content = Cache.Get( key ) as string;


      if ( content == null )
      {

        content = LoadContent( file );
        var dependency = provider.GetCacheDependency( virtualPath, new[] { virtualPath }, DateTime.UtcNow ) ?? new CacheDependency( HostingEnvironment.MapPath( virtualPath ) );


        Cache.Insert( key, content, dependency );
      }


      return new HtmlContentResult( this, content, new Uri( VirtualPathUtility.ToAbsolute( virtualPath ) ), key );
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


  public class AspxFileLoader : IHtmlContentProvider
  {

    private static readonly string[] allowsExtensions = new[] { ".aspx" };

    public HtmlContentResult LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      virtualPath = VirtualPathUtility.ToAppRelative( virtualPath );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;



      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      using ( var writer = new StringWriter() )
      {
        context.Server.Execute( virtualPath, writer, false );

        return new HtmlContentResult( this, writer.ToString(), new Uri( VirtualPathUtility.ToAbsolute( virtualPath ) ) );
      }
    }
  }

}
