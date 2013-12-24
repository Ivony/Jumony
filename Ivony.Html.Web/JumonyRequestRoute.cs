using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 辅助实现 RequestMapping 机制的路由
  /// </summary>
  public class JumonyRequestRoute : RouteBase, IHtmlRequestRoute
  {

    public static string VirtualPathToken
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


      var virtualPath = httpContext.Request.CurrentExecutionFilePath;

      if ( !VirtualPathProvider.FileExists( virtualPath ) )//文件不存在时不路由
        return null;

      var handler = HtmlHandlerProvider.GetHandler( virtualPath );
      if ( handler == null )
        return null;

      var routeData = new RouteData( this, new JumonyRouteHandler() );
      routeData.Values[HtmlHandlerProvider.HtmlHandlerRouteKey] = handler;

      return routeData;

    }


    protected VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
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



    private static readonly JumonyRequestRoute _instance = new JumonyRequestRoute();

    public static JumonyRequestRoute Instance
    {
      get { return _instance; }
    }

    /// <summary>
    /// 根据当前 HTTP 请求和需要处理的虚拟路径，创建 RequestContext 对象
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>创建的 RequestContetx 对象</returns>
    public static RequestContext CreateRequestContext( HttpContextBase context, string virtualPath )
    {
      var routeData = new RouteData( Instance, JumonyRouteHandler.Instance );
      routeData.DataTokens[VirtualPathToken] = virtualPath;
      return new RequestContext( context, routeData );
    }
  }
}
