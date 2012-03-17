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
    /// <returns></returns>
    public static IHtmlDocument Parse( this IHtmlParser parser, HtmlContentResult content )
    {
      return parser.Parse( content.Content, content.ContentUri );
    }



    /// <summary>
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>HTML 内容加载结果</returns>
    public static HtmlContentResult LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new ArgumentException( "virtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", "virtualPath" );


      lock ( _contentProvidersSync )
      {
        foreach ( var provider in ContentProviders )
        {
          var result = provider.LoadContent( context, virtualPath );

          if ( result != null )
            return result;
        }
      }


      return null;
    }


    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {
      string cacheKey;

      return LoadDocument( context, virtualPath, out cacheKey );
    }

    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <param name="cacheKey">若文档已被缓存，获取缓存键</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath, out string cacheKey )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new ArgumentException( "virtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", "virtualPath" );


      cacheKey = null;

      try
      {
        var provider = BuildManager.CreateInstanceFromVirtualPath( virtualPath, typeof( IHtmlDocumentProvider ) ) as IHtmlDocumentProvider;
        if ( provider != null )
          return provider.CreateDocument();
      }
      catch
      { 
      
      }


      var content = LoadContent( context, virtualPath );
      if ( content == null )
        return null;

      cacheKey = content.CacheKey;

      return ParseDocument( context, content );
    }


    /// <summary>
    /// 获取用于分析 HTML 文档的分析器
    /// </summary>
    /// <param name="context">当前请求上下文</param>
    /// <param name="contentUri">内容的地址</param>
    /// <param name="htmlContent">HTML 文档内容</param>
    /// <returns>HTML 分析器相关信息</returns>
    public static HtmlParserResult GetParser( HttpContextBase context, Uri contentUri, string htmlContent )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( contentUri != null && !contentUri.IsAbsoluteUri )
        throw new ArgumentException( "contentUri只能为null或绝对URI", "contentUri" );




      lock ( _parserProvidersSync )
      {
        foreach ( var provider in ParserProviders )
        {
          var result = provider.GetParser( context, contentUri, htmlContent );

          if ( result != null )
          {
            result.Provider = provider;
            return result;
          }
        }
      }


      //默认行为
      return DefaultParserProvider.GetParser( context, contentUri, htmlContent );
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
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="contentResult">文档加载结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, HtmlContentResult contentResult )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( contentResult == null )
        throw new ArgumentNullException( "contentResult" );



      var result = GetParser( context, contentResult.ContentUri, contentResult.Content );


      return ParseDocument( context, contentResult, result );
    }


    /// <summary>
    /// 分析 HTML 文档，此方法会根据情况缓存文档模型
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="contentResult">文档加载结果</param>
    /// <param name="parserResult">解析器选择结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, HtmlContentResult contentResult, HtmlParserResult parserResult )
    {
      if ( contentResult.CacheKey != null && parserResult.DomProvider != null )//如果可以缓存
      {
        var key = contentResult.CacheKey;
        var cacheKey = string.Format( CultureInfo.InvariantCulture, DocumentCacheKey, contentResult.ContentUri.AbsoluteUri );

        var createDocument = Cache.Get( cacheKey ) as Func<IHtmlDomProvider, IHtmlDocument>;

        if ( createDocument != null )
        {
          var provider = parserResult.DomProvider;
          return createDocument( provider );
        }

        context.Trace.Write( "Jumony for ASP.NET", "Document cache missed" );


        var document = ParseDocument( parserResult, contentResult.Content, contentResult.ContentUri );
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

        return ParseDocument( parserResult, contentResult.Content, contentResult.ContentUri );
    }


    /// <summary>
    /// 分析 HTML 文档，此方法永不缓存
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="htmlContent">文档内容</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, string htmlContent, Uri contentUri )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( htmlContent == null )
        throw new ArgumentNullException( "htmlContent" );

      if ( contentUri != null && !contentUri.IsAbsoluteUri )
        throw new ArgumentException( "contentUri只能为null或绝对URI", "contentUri" );


      var result = GetParser( context, contentUri, htmlContent );

      return ParseDocument( result, htmlContent, contentUri );
    }


    private static IHtmlDocument ParseDocument( HtmlParserResult result, string htmlContent, Uri url )
    {
      var parser = result.Parser;

      var document = parser.Parse( htmlContent, url );

      if ( result.Provider != null )
        result.Provider.ReleaseParser( parser );

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
}
