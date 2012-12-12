using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <param name="document">要处理的文档</param>
    void ProcessDocument( HttpContextBase context, IHtmlDocument document );

  }
}
