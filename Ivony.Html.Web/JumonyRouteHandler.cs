using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 映射 HTML 文件路由请求到 HTML 处理器
  /// </summary>
  public class JumonyRouteHandler : IRouteHandler
  {

    public IHttpHandler GetHttpHandler( RequestContext requestContext )
    {

      return new JumonyHandler();

    }


    private static readonly JumonyRouteHandler instance = new JumonyRouteHandler();

    public static IRouteHandler Instance { get { return instance; } }
  }
}
