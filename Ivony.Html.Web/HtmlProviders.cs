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

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供 Jumony Web 所有提供程序的管理和注册的静态类
  /// </summary>
  public static class HtmlProviders
  {



    /// <summary>
    /// 映射请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求信息</param>
    /// <returns>请求映射信息</returns>
    public static RequestMapping MapRequest( HttpRequestBase request )
    {

      if ( request == null )
        throw new ArgumentNullException( "request" );



      var virtualPath = request.GetVirtualPath();

      foreach ( var mapper in WebServices.GetServices<IRequestMapper>( virtualPath ).Concat( Default.RequestMappers ) )
      {
        var result = mapper.MapRequest( request );
        if ( result != null )
        {
          result.Mapper = mapper;
          return result;
        }
      }

      return null;
    }



    /// <summary>
    /// 分析文档内容
    /// </summary>
    /// <param name="parser">分析器</param>
    /// <param name="content">文档内容加载结果</param>
    /// <returns>分析结果</returns>
    public static IHtmlDocument Parse( this IHtmlParser parser, HtmlContentResult content )
    {
      return parser.Parse( content.Content, CreateDocumentUri( content.VirtualPath ) );
    }



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



      var services = WebServices.GetServices<IHtmlContentProvider>( virtualPath ).Concat( Default.GetContentServices( virtualPath ) );
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



    internal static DefaultProviders Default = new DefaultProviders();


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



      foreach ( var provider in WebServices.GetServices<IHtmlParserProvider>( contentResult.VirtualPath ) )
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

        Trace( "Document cache missed" );


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

    private static void Trace( string message )
    {
      HttpContext.Current.Trace.Write( "Jumony for ASP.NET", message );
    }


    private static readonly Uri baseUri = new Uri( "virtualpath://" + Guid.NewGuid().ToString( "N" ) + "/" );

    private static Uri CreateDocumentUri( string virtualPath )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlProviders.VirtualPathFormatError( "virtualPath" );

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

      var virtualPath = context.Request.GetVirtualPath();

      foreach ( var provider in WebServices.GetServices<ICachePolicyProvider>( virtualPath ) )
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




  /// <summary>
  /// ContentProvider 容器
  /// </summary>
  public class ContentProviderCollection
  {

    Dictionary<string, IHtmlContentProvider> data = new Dictionary<string, IHtmlContentProvider>( StringComparer.OrdinalIgnoreCase );


    /// <summary>
    /// 所有支持的扩展名
    /// </summary>
    public string[] SupportedExtensions
    {
      get { return data.Keys.ToArray(); }
    }

    /// <summary>
    /// 检测是否能加载指定虚拟路径的文档内容
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>是否能加载指定虚拟路径的文档内容</returns>
    public bool CanLoadContent( string virtualPath )
    {
      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlProviders.VirtualPathFormatError( "virtualPath" );

      return SupportedExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) );
    }

    /// <summary>
    /// 获取指定虚拟路径的文档内容加载程序
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns></returns>
    public IHtmlContentProvider GetProvider( string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlProviders.VirtualPathFormatError( "virtualPath" );

      var extensions = VirtualPathUtility.GetExtension( virtualPath );
      IHtmlContentProvider provider;
      lock ( _sync )
      {
        if ( data.TryGetValue( extensions, out provider ) )
          return provider;

        else
          return null;
      }
    }


    /// <summary>
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns></returns>
    public HtmlContentResult LoadContent( string virtualPath )
    {
      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlProviders.VirtualPathFormatError( "virtualPath" );

      var provider = GetProvider( virtualPath );
      return provider.LoadContent( virtualPath );

    }


    /// <summary>
    /// 注册一个 HTML 内容提供程序
    /// </summary>
    /// <param name="extension">所支持的扩展名</param>
    /// <param name="provider">HTML 内容提供程序</param>
    public void RegisterContentProvider( string extension, IHtmlContentProvider provider )
    {

      lock ( _sync )
      {
        if ( data.ContainsKey( extension ) )
          throw new InvalidOperationException( "该扩展名已经被注册了" );

        data.Add( extension, provider );
      }

    }

    private object _sync = new object();
  }
}
