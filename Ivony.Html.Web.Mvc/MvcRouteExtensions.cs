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
  }
}
