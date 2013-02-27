using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Ivony.Web;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 关于 MVC 路由的一些扩展
  /// </summary>
  public static class MvcRouteExtensions
  {

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
        var routeTable = routes.OfType<SimpleRouteTable>().FirstOrDefault( route => route.Name == "Mvc_BuiltIn" );
        if ( routeTable == null )
          routes.Add( routeTable = new SimpleRouteTable( "Mvc_BuiltIn", new MvcRouteHandler(), true ) );
        return routeTable;
      }
    }

    /// <summary>
    /// 获取指定区域的简单区域路由表实例，如果没有则创建一个。
    /// </summary>
    /// <param name="context">区域注册上下文</param>
    /// <returns>内建的简单区域路由表实例</returns>
    public static SimpleAreaRouteTable SimpleRouteTable( this AreaRegistrationContext context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );


      var routes = context.Routes;
      var areaName = context.AreaName;
      var namespaces = context.Namespaces.IfNull( new string[0], n => n.ToArray() );
      var useNamespaceFallback = namespaces.Length == 0;

      lock ( routes )
      {
        var routeTable = routes.OfType<SimpleAreaRouteTable>().FirstOrDefault( route => route.AreaName.EqualsIgnoreCase( areaName ) );
        if ( routeTable == null )
          routes.Add( routeTable = new SimpleAreaRouteTable( areaName, namespaces, useNamespaceFallback ) );
        return routeTable;
      }
    }


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
