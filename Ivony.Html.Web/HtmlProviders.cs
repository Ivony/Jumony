using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Web.Caching;

namespace Ivony.Html.Web
{


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

    public static ICollection<IHtmlParserProvider> ParserProviders
    {
      get;
      private set;
    }


    private static readonly object _contentProvidersSync = new object();

    public static ICollection<IHtmlContentProvider> ContentProviders
    {
      get;
      private set;
    }


    private static readonly object _mappersSync = new object();

    public static ICollection<IRequestMapper> RequestMappers
    {
      get;
      private set;
    }


    private static readonly object _cachePoliciesSync = new object();

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


      var content = LoadContent( context, virtualPath );
      if ( content == null )
        return null;

      return ParseDocument( context, virtualPath, content );
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


      if ( contentResult.Cacheable && result.DomProvider != null )//如果可以缓存
      {
        var key = contentResult.CacheKey ?? virtualPath;
        key = string.Format( "HtmlProviders_HtmlDocumentCache_{0}", key );

        var createDocument = Cache.Get( key ) as Func<IHtmlDomProvider, IHtmlDocument>;

        if ( createDocument != null )
          return createDocument( result.DomProvider );



        var document = ParseDocument( result, content );

        createDocument = document.Compile();

        Cache.Insert( key, createDocument, contentResult.CacheDependency );

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

    public static HtmlCachePolicy GetCachePolicy( HttpContextBase context, IHtmlHandler handler, IHtmlDocument document )
    {
      lock ( _cachePoliciesSync )
      {
        foreach ( var provider in CachePolicyProviders )
        {
          var policy = provider.GetPolicy( context, handler, document );
          if ( policy != null )
            return policy;
        }
      }

      return DefaultCachePolicy( context );
    }

    private static HtmlCachePolicy DefaultCachePolicy( HttpContextBase context )
    {
      return new HtmlCachePolicy() { Duration = new TimeSpan( 0, 0, 10 ) };
    }

    private static string DefaultCacheKey( HttpContextBase context )
    {
      return context.Request.Url.AbsoluteUri;
    }
  }

}
