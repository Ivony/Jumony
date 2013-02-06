using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Ivony.Fluent;
using Ivony.Web;

namespace Ivony.Web
{

  /// <summary>
  /// 注册路由的一些扩展方法
  /// </summary>
  public static class RouteExtensions
  {

    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapAction( this SimpleRouteTable routeTable, string urlPattern, string controller, string action, string[] queryKeys = null )
    {
      return routeTable.MapRoute( urlPattern, urlPattern, new Dictionary<string, string>() { { "action", action }, { "controller", controller } }, queryKeys );
    }



    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, object routeValues = null, string[] queryKeys = null )
    {
      return MapRoute( routeTable, urlPattern, routeValues.ToPropertiesMap(), queryKeys );
    }

    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys = null )
    {
      return MapRoute( routeTable, urlPattern, urlPattern, routeValues, queryKeys );
    }



    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="name">路由规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string name, string urlPattern, object routeValues = null, string[] queryKeys = null )
    {
      return MapRoute( routeTable, name, urlPattern, routeValues.ToPropertiesMap(), queryKeys );
    }

    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="name">路由规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys = null )
    {
      if ( routeTable == null )
        throw new ArgumentNullException( "routeTable" );

      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( urlPattern == null )
        throw new ArgumentNullException( "urlPattern" );

      if ( routeValues == null )
        routeValues = new Dictionary<string, string>();


      routeTable.AddRule( name, urlPattern, routeValues, queryKeys );
      return routeTable;
    }
  }
}
