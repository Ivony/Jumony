using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ivony.Html.Parser;
using System.Web.Hosting;



[assembly: PreApplicationStartMethod( typeof( Ivony.Html.Web.Mvc.MvcEnvironment ), "Initialize" )]

namespace Ivony.Html.Web.Mvc
{

  public static class MvcEnvironment
  {
    /// <summary>
    /// 此方法由 ASP.NET 4 系统调用，不应从用户代码中直接调用。
    /// </summary>
    public static void Initialize()
    {
      ViewEngines.Engines.Add( _viewEngine );

      CachePolicyProviders = new SynchronizedCollection<IMvcCachePolicyProvider>( _cachePolicyProvidersSync );
      GlobalCacheFilter = new GlobalCacheFilter();


      CacheStorageProvider = new WebCacheStorageProvider( HostingEnvironment.Cache );
    }



    private static bool _isWarmedUp = false;

    /// <summary>
    /// 调用此方法通知预热 Jumony for MVC 运行环境。
    /// </summary>
    public static void WarmUp()
    {
      if ( !_isWarmedUp )
      {
        CssSelector.WarmUp();
        _isWarmedUp = true;
      }
    }



    /// <summary>
    /// 调用此方法确保路由在简单路由表实例前注册。
    /// </summary>
    /// <param name="route"></param>
    public static void RegisterRouteBeforeSimpleRouteTable( RouteBase route )
    {
      lock ( RouteTable.Routes )
      {
        var index = RouteTable.Routes.IndexOf( SimpleRouteTable );
        if ( index == -1 )
          RouteTable.Routes.Add( route );

        else
          RouteTable.Routes.Insert( index, route );
      }
    }




    private static JumonyViewEngine _viewEngine = new JumonyViewEngine();

    /// <summary>
    /// 获取 Jumony 视图引擎的默认实例
    /// </summary>
    public static JumonyViewEngine JumonyViewEngine
    {
      get { return _viewEngine; }
    }


    /// <summary>
    /// 获取简单路由表的默认内建实例
    /// </summary>
    public static SimpleRouteTable SimpleRouteTable
    {
      get { return RouteTable.Routes.SimpleRouteTable(); }
    }





    private static MvcConfiguration _configuration = new MvcConfiguration();

    /// <summary>
    /// 获取 Jumony for MVC 配置对象，可以对 Jumony for MVC 行为进行调整。
    /// </summary>
    public static MvcConfiguration Configuration
    {
      get { return _configuration; }
    }


    /// <summary>
    /// 当任何一个 JumonyViewEngine 对象成功创建了视图时发生。
    /// </summary>
    public static event EventHandler<JumonyViewEventArgs> ViewCreated;

    internal static void OnViewCreated( object sender, JumonyViewEventArgs e )
    {
      if ( ViewCreated != null )
        ViewCreated( sender, e );
    }




    private static object _cachePolicyProvidersSync = new object();

    /// <summary>
    /// 全局缓存策略提供程序
    /// </summary>
    public static ICollection<IMvcCachePolicyProvider> CachePolicyProviders
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    /// <param name="context">当前请求的 MVC 上下文</param>
    /// <param name="action">请求的 Action</param>
    /// <param name="parameters">Action 的参数信息</param>
    /// <returns></returns>
    public static CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      lock ( _cachePolicyProvidersSync )
      {
        foreach ( var provider in CachePolicyProviders )
        {
          var policy = provider.CreateCachePolicy( context, action, parameters );
          if ( policy != null )
            return policy;
        }

        return DefaultCachePolicyProvider.CreateCachePolicy( context, action, parameters );
      }
    }




    /// <summary>
    /// 使用 MvcParser 分析加载文档。
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>加载的文档对象</returns>
    public static IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {
      return HtmlProviders.LoadDocument( context, virtualPath );
    }




    /// <summary>
    /// 获取默认的缓存储存提供程序
    /// </summary>
    public static ICacheStorageProvider CacheStorageProvider
    {
      get;
      set;
    }





    /// <summary>
    /// 全局缓存筛选器，在 ASP.NET MVC 3 中，将此过滤器加入全局过滤器集合中，即可对所有请求执行输出缓存。
    /// </summary>
    public static GlobalCacheFilter GlobalCacheFilter
    {
      get;
      private set;
    }



    private static readonly IMvcCachePolicyProvider _defaultCachePolicyProvider = new HtmlCachePolicyProvider();

    /// <summary>
    /// 默认的缓存策略，由 HtmlProviders 提供
    /// </summary>
    public static IMvcCachePolicyProvider DefaultCachePolicyProvider
    {
      get { return _defaultCachePolicyProvider; }
    }



    private class HtmlCachePolicyProvider : IMvcCachePolicyProvider
    {
      public CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
      {
        return CreateCachePolicy( context.HttpContext );
      }

      public CachePolicy CreateCachePolicy( HttpContextBase context )
      {
        return HtmlProviders.GetCachePolicy( context );
      }
    }




  }

}
