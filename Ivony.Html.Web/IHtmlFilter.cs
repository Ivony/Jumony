using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 处理程序筛选器，实现此接口并注册类型可以在处理 HTML 文档时插入自定义的逻辑
  /// </summary>
  public interface IHtmlFilter
  {

    /// <summary>
    /// 当处理 HTML 文档之前需要执行的逻辑
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    void OnProcessing( HtmlRequestContext context );

    /// <summary>
    /// 当处理 HTML 文档完毕后需要执行的逻辑
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    void OnProcessed( HtmlRequestContext context );


    /// <summary>
    /// 当渲染 HTML 文档之前需要执行的逻辑
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    void OnRendering( HtmlRequestContext context );

    /// <summary>
    /// 当渲染 HTML 文档完毕后需要执行的逻辑
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    void OnRendered( HtmlRequestContext context );

  }
}
