using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 辅助实现 RequestMapping 机制的路由
  /// </summary>
  public class JumonyRequestMapperRoute : RouteBase
  {
    public static string TokenKey
    {
      get { return "HtmlVirtualPath"; }
    }

    /// <summary>
    /// 获取路由信息，将对请求进行 RequestMapping 的结果包装成路由信息
    /// </summary>
    /// <param name="httpContext">当前 HTTP 请求上下文</param>
    /// <returns>路由信息</returns>
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {

      var mapping = HtmlProviders.MapRequest( httpContext.Request );
      if ( mapping == null )
        return null;



      var routeData = new RouteData( this, JumonyRouteHandler.Instance );
      routeData.DataTokens[JumonyRouteHandler.MappingKey] = mapping;

      return routeData;

    }


    /// <summary>
    /// 获取虚拟路径，此方法总是返回 null
    /// </summary>
    /// <param name="requestContext">当前请求上下文</param>
    /// <param name="values">路由值</param>
    /// <returns>虚拟路径数据，总是返回 null</returns>
    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {
      return null;
    }
  }
}
