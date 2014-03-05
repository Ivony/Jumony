using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 映射 HTML 文件路由请求到 HTML 处理程序
  /// </summary>
  public class JumonyRouteHandler : IRouteHandler
  {


    /// <summary>
    /// 实现 GetHttpHandler 方法，返回 Jumony HTML 页面处理程序
    /// </summary>
    /// <param name="requestContext">当前请求上下文</param>
    /// <returns>返回 Jumony HTML 页面处理程序</returns>
    public IHttpHandler GetHttpHandler( RequestContext requestContext )
    {

      return new JumonyHandler();

    }


    private static readonly JumonyRouteHandler instance = new JumonyRouteHandler();


    /// <summary>
    /// 获取 JumonyRouteHandler 实例
    /// </summary>
    public static IRouteHandler Instance { get { return instance; } }
  }
}
