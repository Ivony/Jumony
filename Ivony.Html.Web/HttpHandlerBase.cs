using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 所有处理 HTTP 请求的处理器基类
  /// </summary>
  public abstract class HttpHandlerBase
  {

    /// <summary>
    /// 获取当前请求的 HTTP 上下文
    /// </summary>
    protected abstract HttpContextBase HttpContext { get; }

    /// <summary>
    /// 获取请求的页的 HttpRequest 对象
    /// </summary>
    protected HttpRequestBase Request
    {
      get { return HttpContext.Request; }
    }


    /// <summary>
    /// 获取 HttpResponse 对象。该对象使您得以将 HTTP 响应数据发送到客户端，并包含有关该响应的信息
    /// </summary>
    protected HttpResponseBase Response
    {
      get { return HttpContext.Response; }
    }


    /// <summary>
    /// 获取 Server 对象，它是 HttpServerUtility 类的实例
    /// </summary>
    protected HttpServerUtilityBase Server
    {
      get { return HttpContext.Server; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 HttpApplicationState 对象
    /// </summary>
    protected HttpApplicationStateBase Application
    {
      get { return HttpContext.Application; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 TraceContext 对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }


    /// <summary>
    /// 获取与该页驻留的应用程序关联的 Cache 对象
    /// </summary>
    protected Cache Cache
    {
      get { return HttpContext.Cache; }
    }


    /// <summary>
    /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径
    /// </summary>
    /// <param name="virtualPath">Web 服务器的虚拟路径</param>
    /// <returns>与 path 相对应的物理文件路径</returns>
    public string MapPath( string virtualPath )
    {
      return Server.MapPath( virtualPath );
    }


  }
}
