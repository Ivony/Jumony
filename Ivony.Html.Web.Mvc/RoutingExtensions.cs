using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  
  /// <summary>
  /// 注册路由的一些扩展方法
  /// </summary>
  public static class RoutingExtensions
  {

    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routingTable">简单路由表对象</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <returns>返回简单路由表对象，便于链式注册</returns>
    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string urlPattern, string controller, string action )
    {
      return MapAction( routingTable, string.Format( "{0}/{1}", controller, action ), urlPattern, controller, action );
    }


    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routingTable">简单路由表对象</param>
    /// <param name="name">路由规则的名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <returns>返回简单路由表对象，便于链式注册</returns>
    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string name, string urlPattern, string controller, string action )
    {
      return MapAction( routingTable, name, urlPattern, controller, action, new string[0] );
    }


    /// <summary>
    /// 映射一个 Action 路由
    /// </summary>
    /// <param name="routingTable">简单路由表对象</param>
    /// <param name="name">路由规则的名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="controller">Controller 名称</param>
    /// <param name="action">Action 名称</param>
    /// <param name="queryKeys">可用于 QueryString 的参数</param>
    /// <returns>返回简单路由表对象，便于链式注册</returns>
    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string name, string urlPattern, string controller, string action, string[] queryKeys )
    {
      routingTable.AddRule( name, urlPattern, new Dictionary<string, string>() { { "action", action }, { "controller", controller } }, queryKeys );

      return routingTable;
    }


  }
}
