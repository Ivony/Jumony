using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Ivony.Fluent;
using Ivony.Html.Parser;
using Ivony.Web;



[assembly: PreApplicationStartMethod( typeof( Ivony.Html.Web.MvcEnvironment ), "Initialize" )]

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供 Jumony for MVC 的环境支持
  /// </summary>
  public static class MvcEnvironment
  {
    /// <summary>
    /// 此方法由 ASP.NET 4 系统调用，不应从用户代码中直接调用。
    /// </summary>
    public static void Initialize()
    {
      ViewEngines.Engines.Add( _viewEngine );

      CacheStorageProvider = new WebCacheStorageProvider( HostingEnvironment.Cache );
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
      get { return Routes.SimpleRouteTable(); }
    }


    private static RouteCollection Routes
    {
      get { return System.Web.Routing.RouteTable.Routes; }
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





    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    /// <param name="context">当前请求的 MVC 上下文</param>
    /// <param name="action">请求的 Action</param>
    /// <param name="parameters">Action 的参数信息</param>
    /// <returns></returns>
    public static CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      foreach ( var provider in WebServiceLocator.GetServices<IMvcCachePolicyProvider>() )
      {
        var policy = provider.CreateCachePolicy( context, action, parameters );
        if ( policy != null )
          return policy;
      }


      if ( Configuration.FallbackCachePolicy )
        return FallbackCachePolicyProvider.CreateCachePolicy( context, action, parameters );

      else
        return null;
    }




    /// <summary>
    /// 获取默认的缓存储存提供程序
    /// </summary>
    public static ICacheStorageProvider CacheStorageProvider
    {
      get;
      set;
    }





    private static readonly IMvcCachePolicyProvider _fallbackCache = new FallbackCachePolicyProvider();

    /// <summary>
    /// 回溯到传统的缓存策略，如果要获取当前请求的传统缓存策略，则使用这个缓存策略提供程序。
    /// </summary>
    public static IMvcCachePolicyProvider FallbackCachePolicyProvider
    {
      get { return _fallbackCache; }
    }





    /// <summary>
    /// 获取当前 Controller 所属的 Area
    /// </summary>
    /// <remarks>注意，即使当前请求属于某个 Area ，如果执行的 Controller 的类型不属于这个区域的命名空间，此方法仍然会返回 null （即执行的是无区域的 Controller）</remarks>
    /// <param name="context">当前执行的控制器上下文</param>
    /// <returns>所属的 Area</returns>
    public static string GetAreaName( ControllerContext context )
    {
      var areaName = context.RouteData.DataTokens["area"] as string;
      if ( areaName == null )
      {
        var route = context.RouteData.Route as IRouteWithArea;
        if ( route == null )
          return null;

        areaName = route.Area;
      }

      var namespaces = context.RouteData.DataTokens["Namespaces"] as string[];
      if ( namespaces == null )
        return areaName;

      var controllerType = context.Controller.GetType();
      if ( namespaces.Any( n => InNamespace( n, controllerType.Namespace ) ) )
        return areaName;

      return null;

    }

    private static bool InNamespace( string namespaceSetting, string controllerNamespace )
    {

      if ( string.IsNullOrEmpty( controllerNamespace ) )
        return false;

      if ( namespaceSetting.EndsWith( ".*" ) )
        return controllerNamespace.StartsWith( namespaceSetting.Substring( 0, namespaceSetting.Length - 2 ), StringComparison.OrdinalIgnoreCase );
      else
        return controllerNamespace.EqualsIgnoreCase( namespaceSetting );
    }
  }
}
