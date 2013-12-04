using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 定义一个 HTML 文档处理程序
  /// </summary>
  public interface IHtmlHandler : IDisposable
  {

    /// <summary>
    /// 处理 HTML 文档
    /// </summary>
    /// <param name="context">当前请求上下文</param>
    void ProcessScope( HtmlRequestContext context );

  }



  /// <summary>
  /// 定义一个异步的 HTML 文档处理程序
  /// </summary>
  public interface IAsyncHtmlHandler : IDisposable
  {

    /// <summary>
    /// 异步处理 HTML 文档
    /// </summary>
    /// <param name="context">当前请求上下文</param>
    /// <returns>管理异步处理请求的任务</returns>
    Task ProcessScopeAsync( HtmlRequestContext context );
  }

}
