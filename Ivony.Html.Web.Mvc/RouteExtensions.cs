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
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapAction( this SimpleRouteTable routeTable, string urlPattern, string controller, string action )
    {
      return MapAction( routeTable, urlPattern, controller, action, null );
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
    public static SimpleRouteTable MapAction( this SimpleRouteTable routeTable, string urlPattern, string controller, string action, string[] queryKeys )
    {
      AddRule( routeTable, urlPattern, controller, action, queryKeys );

      return routeTable;
    }


    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="context">路由区域信息</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapAction( this SimpleRouteTable routeTable, AreaRegistrationContext context, string urlPattern, string controller, string action )
    {
      return routeTable.MapAction( context, urlPattern, controller, action, null );
    }


    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="context">路由区域信息</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapAction( this SimpleRouteTable routeTable, AreaRegistrationContext context, string urlPattern, string controller, string action, string[] queryKeys )
    {
      var rule = AddRule( routeTable, urlPattern, controller, action, queryKeys );

      var namespaces = context.Namespaces.ToArray();

      rule.DataTokens["Namespaces"] = namespaces;
      rule.DataTokens["area"] = context.AreaName;
      bool flag = namespaces == null || namespaces.Length == 0;
      rule.DataTokens["UseNamespaceFallback"] = flag;

      return routeTable;
    }



    private static SimpleRouteRule AddRule( SimpleRouteTable routeTable, string urlPattern, string controller, string action, string[] queryKeys )
    {
      return AddRule( routeTable, urlPattern, urlPattern, new Dictionary<string, string>() { { "action", action }, { "controller", controller } }, queryKeys );
    }


    private static SimpleRouteRule AddRule( SimpleRouteTable routeTable, string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {
      return routeTable.AddRule( name ?? urlPattern, urlPattern, routeValues, queryKeys ?? new string[0] );
    }


    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, object routeValues )
    {
      return MapRoute( routeTable, urlPattern, routeValues.ToPropertiesMap(), null );
    }

    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, IDictionary<string, string> routeValues )
    {
      return MapRoute( routeTable, urlPattern, routeValues, null );
    }




    /// <summary>
    /// 映射一个路由规则
    /// </summary>
    /// <param name="routeTable">简单路由表实例</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">默认/静态路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的路由值</param>
    /// <returns>返回简单路由表实例，便于链式注册</returns>
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, object routeValues, string[] queryKeys )
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
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {
      return MapRoute( routeTable, null, urlPattern, routeValues, queryKeys );
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
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string name, string urlPattern, object routeValues, string[] queryKeys )
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
    public static SimpleRouteTable MapRoute( this SimpleRouteTable routeTable, string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {
      AddRule( routeTable, name, urlPattern, routeValues, queryKeys );
      return routeTable;
    }


    /// <summary>
    /// 获取内建的简单路由表实例，如果没有则创建一个。
    /// </summary>
    /// <param name="routes">系统路由集合</param>
    /// <returns>内建的简单路由表实例</returns>
    public static SimpleRouteTable SimpleRouteTable( this RouteCollection routes )
    {
      lock ( routes )
      {
        var routeTable = routes.OfType<SimpleRouteTable>().FirstOrDefault( route => route.IsBuiltIn );
        if ( routeTable == null )
          routes.Add( routeTable = new SimpleRouteTable() );
        return routeTable;
      }
    }




  }
}
