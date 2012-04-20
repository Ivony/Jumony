using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
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
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, object routeValues, string[] queryKeys = null )
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
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string name, string urlPattern, object routeValues, string[] queryKeys = null )
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
        throw new ArgumentNullException( "routeValues" );


      routeTable.AddRule( name, urlPattern, routeValues, queryKeys );
      return routeTable;
    }


    /// <summary>
    /// 获取内建的简单路由表实例，如果没有则创建一个。
    /// </summary>
    /// <param name="routes">系统路由集合</param>
    /// <returns>内建的简单路由表实例</returns>
    public static SimpleRouteTable SimpleRouteTable( this RouteCollection routes )
    {
      if ( routes == null )
        throw new ArgumentNullException( "routes" );

      lock ( routes )
      {
        var routeTable = routes.OfType<SimpleRouteTable>().FirstOrDefault( route => route.IsBuiltIn );
        if ( routeTable == null )
          routes.RegisterSimpleRouteTable( routeTable = new SimpleRouteTable() );
        return routeTable;
      }
    }

    /// <summary>
    /// 获取指定区域的简单区域路由表实例，如果没有则创建一个。
    /// </summary>
    /// <param name="context">区域注册上下文</param>
    /// <returns>内建的简单区域路由表实例</returns>
    public static SimpleAreaRouteTable SimpleAreaRouteTable( this AreaRegistrationContext context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );


      var routes = context.Routes;
      var areaName = context.AreaName;
      var namespaces = context.Namespaces.ToArray();
      var useNamespaceFallback = namespaces == null || namespaces.Length == 0;

      lock ( routes )
      {
        var routeTable = routes.OfType<SimpleAreaRouteTable>().FirstOrDefault( route => route.AreaName.EqualsIgnoreCase( areaName ) );
        if ( routeTable == null )
          routes.RegisterSimpleRouteTable( routeTable = new SimpleAreaRouteTable( areaName, namespaces, useNamespaceFallback ) );
        return routeTable;
      }
    }


    /// <summary>
    /// 检查规则是否与任何简单路由表冲突
    /// </summary>
    /// <param name="routes">路由集合</param>
    /// <param name="rule">要检查的简单路由规则</param>
    /// <returns>如果有冲突，返回冲突的规则</returns>
    public static SimpleRouteRule CheckConflict( this RouteCollection routes, SimpleRouteRule rule )
    {

      if ( routes == null )
        throw new ArgumentNullException( "routes" );

      if ( rule == null )
        throw new ArgumentNullException( "rule" );


      lock ( routes )
      {
        return routes.OfType<SimpleRouteTable>().Select( routeTable => routeTable.CheckConflict( rule ) ).NotNull().FirstOrDefault();
      }
    }


    /// <summary>
    /// 向路由集合中注册一个简单路由表实例
    /// </summary>
    /// <param name="routes">路由集合</param>
    /// <param name="routeTable">简单路由表实例</param>
    /// <returns>路由集合</returns>
    public static RouteCollection RegisterSimpleRouteTable( this RouteCollection routes, SimpleRouteTable routeTable )
    {

      if ( routes == null )
        throw new ArgumentNullException( "routes" );

      if ( routeTable == null )
        throw new ArgumentNullException( "routeTable" );


      lock ( routes )
      {
        if ( routeTable.Routes == routes )
          return routes;

        routes.Add( routeTable );
        routeTable.Routes = routes;
      }

      return routes;
    }

  }
}
