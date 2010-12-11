using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public interface IHtmlHandlerProvider
  {

    /// <summary>
    /// 获取 HTML 处理程序
    /// </summary>
    /// <param name="context">当前 Http 请求信息</param>
    /// <param name="virtualPath">HTML 文档的虚路径</param>
    /// <returns>HTML 处理程序</returns>
    IHtmlHandler GetHandler( HttpContextBase context, string virtualPath );

    /// <summary>
    /// 获取 HTML 处理程序
    /// </summary>
    /// <param name="context">当前 Http 请求路由信息</param>
    /// <param name="virtualPath">HTML 文档的虚路径</param>
    /// <returns>HTML 处理程序</returns>
    IHtmlHandler GetHandler( RequestContext context, string virtualPath );

  }
}
