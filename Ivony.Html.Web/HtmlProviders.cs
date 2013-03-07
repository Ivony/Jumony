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

    static HtmlProviders()
    {
      ParserProviders = new SynchronizedCollection<IHtmlParserProvider>( _parserProvidersSync );
      ContentProviders = new SynchronizedCollection<IHtmlContentProvider>( _contentProvidersSync );
      RequestMappers = new SynchronizedCollection<IRequestMapper>( _mappersSync );
      CachePolicyProviders = new SynchronizedCollection<ICachePolicyProvider>( _cachePoliciesSync );


      ContentProviders.Add( new StaticFileLoader() );
      ContentProviders.Add( new AspxFileLoader() );

      RequestMappers.Add( new DefaultRequestMapper() );
    }


    private static readonly object _parserProvidersSync = new object();

    /// <summary>
    /// 所有解析器提供程序
    /// </summary>
    public static ICollection<IHtmlParserProvider> ParserProviders
    {
      get;
      private set;
    }


    private static readonly object _contentProvidersSync = new object();

    /// <summary>
    /// 所有内容提供程序
    /// </summary>
    public static ICollection<IHtmlContentProvider> ContentProviders
    {
      get;
      private set;
    }


    private static readonly object _mappersSync = new object();

    /// <summary>
    /// 所有请求映射提供程序
    /// </summary>
    public static ICollection<IRequestMapper> RequestMappers
    {
      get;
      private set;
    }


    private static readonly object _cachePoliciesSync = new object();

    /// <summary>
    /// 所有缓存策略提供程序
    /// </summary>
    public static ICollection<ICachePolicyProvider> CachePolicyProviders
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取默认的缓存策略提供程序
    /// </summary>
    public static ICachePolicyProvider DefaultCachePolicyProvider
    {
      get { return Web.DefaultCachePolicyProvider.Instance; }
    }



    /// <summary>
    /// 映射请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求信息</param>
    /// <returns>请求映射信息</returns>
    public static RequestMapping MapRequest( HttpRequestBase request )
    {

      if ( request == null )
        throw new ArgumentNullException( "request" );



      lock ( _mappersSync )
      {
        foreach ( var mapper in RequestMappers )
        {
          var result = mapper.MapRequest( request );
          if ( result != null )
          {
            result.Mapper = mapper;
            return result;
          }
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


      lock ( _contentProvidersSync )
      {
        foreach ( var provider in ContentProviders )
        {
          var result = provider.LoadContent( virtualPath );

          if ( result != null )
            return result;
        }
      }


      return null;
    }


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
    public static HtmlParserResult GetParser( HtmlContentResult contentResult )
    {

      if ( contentResult == null )
        throw new ArgumentNullException( "contentResult" );




      lock ( _parserProvidersSync )
      {
        foreach ( var provider in ParserProviders )
        {
          var result = provider.GetParser( contentResult.VirtualPath, contentResult.Content );

          if ( result != null )
            return result;

        }
      }


      //默认行为
      return DefaultParserProvider.GetParser( contentResult.VirtualPath, contentResult.Content );
    }


    private static IHtmlParserProvider _defaultParserProvider = new DefaultParserProvider();

    /// <summary>
    /// 获取默认的 HTML 解析器
    /// </summary>
    public static IHtmlParserProvider DefaultParserProvider
    {
      get { return _defaultParserProvider; }
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



      var result = GetParser( contentResult );


      return ParseDocument( contentResult, result );
    }


    /// <summary>
    /// 分析 HTML 文档，此方法会根据情况缓存文档模型
    /// </summary>
    /// <param name="contentResult">文档加载结果</param>
    /// <param name="parserResult">解析器选择结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HtmlContentResult contentResult, HtmlParserResult parserResult )
    {
      if ( contentResult.CacheKey != null && parserResult.DomProvider != null )//如果可以缓存
      {
        var key = contentResult.CacheKey;
        var cacheKey = string.Format( CultureInfo.InvariantCulture, DocumentCacheKey, contentResult.VirtualPath );

        var createDocument = Cache.Get( cacheKey ) as Func<IHtmlDomProvider, IHtmlDocument>;

        if ( createDocument != null )
        {
          var provider = parserResult.DomProvider;
          return createDocument( provider );
        }

        Trace( "Document cache missed" );


        var document = ParseDocument( parserResult, contentResult.Content, contentResult.VirtualPath );
        createDocument = document.Compile();//必须同步编译文档，否则文档对象可能被修改。

        new Action( delegate
        {
          createDocument( parserResult.DomProvider );//可以异步预热，预热后再存入缓存。
          Cache.Insert( cacheKey, createDocument, new CacheDependency( new string[0], new[] { key } ), CacheItemPriority.High );
        }
          ).BeginInvoke( null, null );//立即在新线程预热此方法



        return document;
      }

      else

        return ParseDocument( parserResult, contentResult.Content, contentResult.VirtualPath );
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


    private static IHtmlDocument ParseDocument( HtmlParserResult result, string htmlContent, string virtualPath )
    {
      var parser = result.Parser;

      var document = parser.Parse( htmlContent, CreateDocumentUri( virtualPath ) );

      if ( result.Provider != null )
        result.Provider.ReleaseParser( result );

      return document;
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
      lock ( _cachePoliciesSync )
      {
        foreach ( var provider in CachePolicyProviders )
        {
          CachePolicy policy = provider.CreateCachePolicy( context );
          if ( policy != null )
            return policy;
        }

      }

      return Web.DefaultCachePolicyProvider.Instance.CreateCachePolicy( context );
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
