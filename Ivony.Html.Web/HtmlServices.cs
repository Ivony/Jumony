using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Web.Caching;
using System.Globalization;
using System.Web.Compilation;
using System.Web.Configuration;
using Ivony.Fluent;
using System.Collections.ObjectModel;
using Ivony.Web;
using System.Diagnostics;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供 Jumony Web 所有提供程序的管理和注册的静态类
  /// </summary>
  public static class HtmlServices
  {





    /// <summary>
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>HTML 内容加载结果</returns>
    public static HtmlContentResult LoadContent( string virtualPath )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );



      var services = WebServiceLocator.GetServices<IHtmlContentProvider>( virtualPath ).Concat( Default.GetContentServices( virtualPath ) );
      foreach ( var provider in services )
      {
        var result = provider.LoadContent( virtualPath );

        if ( result != null )
        {
          result.Provider = provider;
          result.VirtualPath = virtualPath;
          return result;
        }
      }


      return null;
    }


    /// <summary>
    /// 提供所有默认服务的对象，此属性仅供测试用途。
    /// </summary>
    public static readonly DefaultProviders Default = new DefaultProviders();


    internal static Exception VirtualPathFormatError( string paramName )
    {
      return new ArgumentException( string.Format( CultureInfo.InvariantCulture, "{0} 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", paramName ), paramName );
    }



    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( string virtualPath )
    {
      string cacheKey;

      return LoadDocument( virtualPath, out cacheKey );
    }



    private static HashSet<string> failedHtmlProviders = new HashSet<string>();


    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <param name="cacheDependency">若文档已被缓存，获取缓存依赖项</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( string virtualPath, out CacheDependency cacheDependency )
    {
      string cacheKey;
      var result = LoadDocument( virtualPath, out cacheKey );


      if ( result == null )
      {
        cacheDependency = null;
        return result;
      }


      if ( cacheKey != null )
        cacheDependency = new CacheDependency( new string[0], new string[] { cacheKey } );

      else
        cacheDependency = HtmlServices.CreateCacheDependency( virtualPath );

      return result;
    }


    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <param name="cacheKey">若文档已被缓存，获取缓存键</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( string virtualPath, out string cacheKey )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );


      cacheKey = null;

      var provider = TryGetDocumentProvider( virtualPath );
      if ( provider != null )
        return provider.CreateDocument();

      var content = LoadContent( virtualPath );
      if ( content == null )
        return null;

      cacheKey = content.CacheKey;

      return ParseDocument( content );
    }

    /// <summary>
    /// 尝试获取指定地址 HTML 文档的文档对象提供程序
    /// </summary>
    /// <param name="virtualPath">HTML 文档虚拟路径</param>
    /// <returns>若添加了 HtmlDocumentProvider 设置，并能将文档编译为 IHtmlDocumentProvider 对象，则返回，否则返回 null</returns>
    public static IHtmlDocumentProvider TryGetDocumentProvider( string virtualPath )
    {
      if ( failedHtmlProviders.Contains( virtualPath ) )
        return null;

      var section = WebConfigurationManager.GetSection( "system.web/compilation", virtualPath ) as CompilationSection;
      if ( section == null )
        return null;

      if ( section.BuildProviders[VirtualPathUtility.GetExtension( virtualPath )] == null )
        return null;

      try
      {
        var provider = BuildManager.CreateInstanceFromVirtualPath( virtualPath, typeof( IHtmlDocumentProvider ) ) as IHtmlDocumentProvider;
        if ( provider != null )
          return provider;
      }
      catch
      {
      }

      failedHtmlProviders.Add( virtualPath );
      return null;

    }

    /// <summary>
    /// 获取用于分析 HTML 文档的分析器
    /// </summary>
    /// <param name="contentResult">文档内容加载结果</param>
    /// <returns>HTML 分析器相关信息</returns>
    public static IHtmlParser GetParser( HtmlContentResult contentResult )
    {

      if ( contentResult == null )
        throw new ArgumentNullException( "contentResult" );



      foreach ( var provider in WebServiceLocator.GetServices<IHtmlParserProvider>( contentResult.VirtualPath ) )
      {
        var parser = provider.GetParser( contentResult.VirtualPath, contentResult.Content );
        if ( parser != null )
          return parser;
      }

      return Default.GetParser( contentResult.VirtualPath );
    }




    private const string DocumentCacheKey = "HtmlProviders_HtmlDocumentCache_{0}";

    /// <summary>
    /// 分析 HTML 文档，此方法会根据情况缓存文档模型
    /// </summary>
    /// <param name="contentResult">文档加载结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HtmlContentResult contentResult )
    {

      if ( contentResult == null )
        throw new ArgumentNullException( "contentResult" );

      var parser = GetParser( contentResult );
      return ParseDocument( contentResult, parser );

    }


    /// <summary>
    /// 分析 HTML 文档，此方法会根据情况缓存文档模型
    /// </summary>
    /// <param name="contentResult">文档加载结果</param>
    /// <param name="parser">HTML 解析器</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HtmlContentResult contentResult, IHtmlParser parser )
    {

      var domProvider = parser.DomProvider;

      if ( contentResult.CacheKey != null && domProvider != null )//如果可以缓存
      {
        var key = contentResult.CacheKey;
        var cacheKey = string.Format( CultureInfo.InvariantCulture, DocumentCacheKey, contentResult.VirtualPath );

        var createDocument = Cache.Get( cacheKey ) as Func<IHtmlDomProvider, IHtmlDocument>;

        if ( createDocument != null )
        {
          return createDocument( domProvider );
        }

        WebServiceLocator.GetTraceService().Trace( TraceLevel.Info, "Jumony Web", "Document cache missed" );


        var document = ParseDocument( parser, contentResult.Content, contentResult.VirtualPath );
        createDocument = document.Compile();//必须同步编译文档，否则文档对象可能被修改。

        new Action( delegate
        {
          createDocument( domProvider );//可以异步预热，预热后再存入缓存。
          Cache.Insert( cacheKey, createDocument, new CacheDependency( new string[0], new[] { key } ), CacheItemPriority.High );
        }
          ).BeginInvoke( null, null );//立即在新线程预热此方法



        return document;
      }

      else

        return ParseDocument( parser, contentResult.Content, contentResult.VirtualPath );
    }



    private static readonly Uri baseUri = new Uri( "virtualpath://" + Guid.NewGuid().ToString( "N" ) + "/" );

    private static Uri CreateDocumentUri( string virtualPath )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlServices.VirtualPathFormatError( "virtualPath" );

      return new Uri( baseUri, VirtualPathUtility.ToAbsolute( virtualPath ) );
    }


    private static IHtmlDocument ParseDocument( IHtmlParser parser, string htmlContent, string virtualPath )
    {
      return parser.Parse( htmlContent, CreateDocumentUri( virtualPath ) );
    }



    private static Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }



    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    /// <param name="context">当前 HTTP 请求</param>
    /// <returns>适用于当前请求的缓存策略</returns>
    public static CachePolicy GetCachePolicy( HttpContextBase context )
    {

      var virtualPath = context.Request.AppRelativeCurrentExecutionFilePath;

      foreach ( var provider in WebServiceLocator.GetServices<ICachePolicyProvider>( virtualPath ) )
      {
        CachePolicy policy = provider.CreateCachePolicy( context );
        if ( policy != null )
          return policy;
      }

      return null;

    }



    /// <summary>
    /// 创建指定虚拟路径文件的缓存依赖项，当文件发生变化时可以清除缓存。
    /// </summary>
    /// <param name="virtualPath">需要监视的文件虚拟路径</param>
    /// <returns>监视路径的缓存依赖项</returns>
    public static CacheDependency CreateCacheDependency( string virtualPath )
    {

      return CreateCacheDependency( HostingEnvironment.VirtualPathProvider, virtualPath );

    }


    /// <summary>
    /// 创建指定虚拟路径文件的缓存依赖项，当文件发生变化时可以清除缓存。
    /// </summary>
    /// <param name="provider">当前所使用的虚拟路径提供程序</param>
    /// <param name="virtualPath">需要监视的文件虚拟路径</param>
    /// <returns>监视路径的缓存依赖项</returns>
    public static CacheDependency CreateCacheDependency( VirtualPathProvider provider, string virtualPath )
    {
      var now = DateTime.UtcNow;

      return provider.GetCacheDependency( virtualPath, new[] { virtualPath }, now ) ?? new CacheDependency( HostingEnvironment.MapPath( virtualPath ) );

    }

  }
}
