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

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供Jumony Web所有提供程序的管理和注册的静态类
  /// </summary>
  public static class HtmlProviders
  {

    static HtmlProviders()
    {
      ParserProviders = new SynchronizedCollection<IHtmlParserProvider>( _parserProvidersSync );
      ContentProviders = new SynchronizedCollection<IHtmlContentProvider>( _contentProvidersSync );
      RequestMappers = new SynchronizedCollection<IRequestMapper>( _mappersSync );
      CachePolicyProviders = new SynchronizedCollection<IHtmlCachePolicyProvider>( _cachePoliciesSync );


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
    public static ICollection<IHtmlCachePolicyProvider> CachePolicyProviders
    {
      get;
      private set;
    }


    /// <summary>
    /// 映射请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求信息</param>
    /// <returns>请求映射信息</returns>
    public static RequestMapResult MapRequest( HttpRequest request )
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
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档内容加载结果</returns>
    public static HtmlContentResult LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );



      lock ( _contentProvidersSync )
      {
        foreach ( var provider in ContentProviders )
        {
          var result = provider.LoadContent( context, virtualPath );

          if ( result != null )
          {
            result.Provider = provider;
            return result;
          }
        }
      }


      return null;
    }

    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );



      var content = LoadContent( context, virtualPath );
      if ( content == null )
        return null;

      return ParseDocument( context, virtualPath, content );
    }


    /// <summary>
    /// 加载一个 Web 页面
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>Web 页面对象</returns>
    public static WebPage LoadPage( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      var content = LoadContent( context, virtualPath );
      if ( content == null )
        return null;

      var document = ParseDocument( context, virtualPath, content );

      return new WebPage( document, new Uri( context.Request.Url, virtualPath ), content.CacheKey );

    }



    /// <summary>
    /// 获取用于分析 HTML 文档的分析器
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="htmlContent">HTML 文档内容</param>
    /// <returns>HTML 分析器相关信息</returns>
    public static HtmlParserResult GetParser( HttpContextBase context, string virtualPath, string htmlContent )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      lock ( _parserProvidersSync )
      {
        foreach ( var provider in ParserProviders )
        {
          var result = provider.GetParser( context, virtualPath, htmlContent );

          if ( result != null )
          {
            result.Provider = provider;
            return result;
          }
        }
      }

      return new HtmlParserResult()
      {
        Parser = new JumonyHtmlParser(),
        DomProvider = new DomProvider(),
      };
    }



    private const string DocumentCacheKey = "HtmlProviders_HtmlDocumentCache_{0}";

    /// <summary>
    /// 分析 HTML 文档，此方法会根据情况缓存文档模型
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="result">文档加载结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, string virtualPath, HtmlContentResult contentResult )
    {

      var content = contentResult.Content;

      var result = GetParser( context, virtualPath, content );


      if ( contentResult.CacheKey != null && result.DomProvider != null )//如果可以缓存
      {
        var key = contentResult.CacheKey;
        var cacheKey = string.Format( CultureInfo.InvariantCulture, DocumentCacheKey, key );

        var createDocument = Cache.Get( key ) as Func<IHtmlDomProvider, IHtmlDocument>;

        if ( createDocument != null )
          return createDocument( result.DomProvider );



        var document = ParseDocument( result, content );

        createDocument = document.Compile();

        Cache.Insert( cacheKey, createDocument, new CacheDependency( new string[0], new[] { key } ) );

        return document;
      }

      else

        return ParseDocument( result, content );
    }


    /// <summary>
    /// 分析 HTML 文档，此方法永不缓存
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="htmlContent">文档内容</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, string virtualPath, string htmlContent )
    {
      var result = GetParser( context, virtualPath, htmlContent );

      return ParseDocument( result, htmlContent );
    }

    private static IHtmlDocument ParseDocument( HtmlParserResult result, string htmlContent )
    {
      var parser = result.Parser;

      var document = parser.Parse( htmlContent );

      if ( result.Provider != null )
        result.Provider.ReleaseParser( parser );

      return document;
    }



    private static Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }





    /// <summary>
    /// 获取缓存键（依据）
    /// </summary>
    /// <param name="context">当前HTTP请求信息</param>
    /// <returns>缓存键，对于可能产生同一结果的请求，应产生同一缓存键</returns>
    public static string GetCacheKey( HttpContextBase context )
    {
      lock ( _cachePoliciesSync )
      {
        foreach ( var policy in CachePolicyProviders )
        {
          string cacheKey = policy.GetCacheKey( context );
          if ( cacheKey != null )
            return cacheKey;
        }
      }

      return DefaultCacheKey( context );

    }

    /// <summary>
    /// 获取缓存策略
    /// </summary>
    /// <param name="context">当前HTTP请求信息</param>
    /// <param name="handler">当前负责处理请求的处理程序</param>
    /// <param name="page">处理后的文档</param>
    /// <returns>缓存策略</returns>
    /// <remarks>缓存策略决定了缓存时间和缓存依赖项</remarks>
    public static HtmlCachePolicy GetCachePolicy( HttpContextBase context, IHtmlHandler handler, WebPage page )
    {
      lock ( _cachePoliciesSync )
      {
        foreach ( var provider in CachePolicyProviders )
        {
          var policy = provider.GetPolicy( context, handler, page );
          if ( policy != null )
            return policy;
        }
      }

      return DefaultCachePolicy( context );
    }

    private static HtmlCachePolicy DefaultCachePolicy( HttpContextBase context )
    {
      return new HtmlCachePolicy() { Duration = DefaultCacheDuration };
    }

    private static string DefaultCacheKey( HttpContextBase context )
    {

      if ( DefaultCacheKeyBasis == Web.CacheKeyBasis.NoCache )
        return null;

      var keys = new List<string>();

      if ( (DefaultCacheKeyBasis & Web.CacheKeyBasis.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          keys.Add( context.Session.SessionID );
      }

      if ( (DefaultCacheKeyBasis & Web.CacheKeyBasis.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          keys.Add( context.User.Identity.Name.Replace( ":", "::" ) );
      }

      if ( (DefaultCacheKeyBasis & Web.CacheKeyBasis.ByUrl) != 0 )
        keys.Add( context.Request.Url.AbsoluteUri );

      return string.Join( ":", keys.ToArray() );
    }


    /// <summary>
    /// 设置默认缓存键的产生依据
    /// </summary>
    public static CacheKeyBasis DefaultCacheKeyBasis
    {
      get;
      set;
    }


    /// <summary>
    /// 获取或设置默认缓存时间
    /// </summary>
    public static TimeSpan DefaultCacheDuration
    {
      get;
      set;
    }


  }



  [Flags]
  public enum CacheKeyBasis
  {

    /// <summary>指定对于所有请求都不要缓存</summary>
    NoCache = 0,

    /// <summary>指定对于所有请求，以url作为缓存依据</summary>
    ByUrl = 1,

    /// <summary>指定对于所有请求，以SessionID作为依据</summary>
    BySession = 2,

    /// <summary>指定对于所有请求，以Identity的Name作为依据</summary>
    ByIdentity = 4,


  }


}
