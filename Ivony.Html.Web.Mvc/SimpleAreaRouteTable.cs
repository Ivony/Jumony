using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ivony.Web
{
  /// <summary>
  /// 用于 MVC 的简单区域路由表，提供某一区域的简单路由服务
  /// </summary>
  public sealed class SimpleAreaRouteTable : SimpleRouteTable, IRouteWithArea
  {


    /// <summary>
    /// 构建简单区域路由表对象
    /// </summary>
    /// <param name="areaName">区域名</param>
    /// <param name="namespaces">区域所要搜索的命名空间</param>
    /// <param name="useNamespaceFallback">是否回溯搜索无 Area 命名空间</param>
    internal SimpleAreaRouteTable( string areaName, string[] namespaces, bool useNamespaceFallback )
      : base( "Area_" + areaName, new MvcRouteHandler(), true )
    {
      AreaName = areaName;
      Namespaces = namespaces;
      UseNamespaceFallback = useNamespaceFallback || true;
    }

    /// <summary>
    /// 获取路由表所适用的区域名
    /// </summary>
    public string AreaName
    {
      get;
      private set;
    }

    string IRouteWithArea.Area
    {
      get { return AreaName; }
    }



    /// <summary>
    /// 区域所要搜索的命名空间
    /// </summary>
    public string[] Namespaces
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否回溯搜索无 Area 命名空间
    /// </summary>
    public bool UseNamespaceFallback
    {
      get;
      private set;
    }




    /// <summary>
    /// 添加一个路由规则
    /// </summary>
    /// <param name="name">规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">静态/默认路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的参数</param>
    /// <returns>创建的简单路由规则</returns>
    /// <remarks>
    /// 简单区域路由表会自动为路由规则增加一个静态路由值 area 保存当前区域名。
    /// </remarks>
    public override SimpleRouteRule AddRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {

      var _routeValues = new Dictionary<string, string>( routeValues, StringComparer.OrdinalIgnoreCase );

      if ( _routeValues.ContainsKey( "area" ) )
        throw new InvalidOperationException( "静态路由值不能包含 area" );

      _routeValues.Add( "area", AreaName );

      return base.AddRule( name, urlPattern, _routeValues, queryKeys );
    }


    /// <summary>
    /// 获取请求的路由数据
    /// </summary>
    /// <param name="httpContext">HTTP 请求</param>
    /// <returns>路由数据</returns>
    /// <remarks>
    /// 简单区域路由表获取路由数据后会自动设置区域所需的 DataTokens
    /// </remarks>
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {
      var routeData = base.GetRouteData( httpContext );

      if ( routeData != null )
      {
        routeData.Values.Remove( "area" );
        routeData.DataTokens["area"] = AreaName;
        routeData.DataTokens["Namespaces"] = Namespaces;
        routeData.DataTokens["UseNamespaceFallback"] = UseNamespaceFallback;
      }

      return routeData;
    }



    /// <summary>
    /// 尝试从路由值创建虚拟路径
    /// </summary>
    /// <param name="requestContext">当前请求上下文</param>
    /// <param name="values">路由值</param>
    /// <remarks>
    /// 简单区域路由表获取路由数据后会自动设置区域所需的 DataTokens
    /// </remarks>
    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {
      values["area"] = AreaName;

      var data = base.GetVirtualPath( requestContext, values );

      if ( data != null )
      {
        data.DataTokens["area"] = AreaName;
        data.DataTokens["Namespaces"] = Namespaces;
        data.DataTokens["UseNamespaceFallback"] = UseNamespaceFallback;
      }

      return data;
    }
  }
}
