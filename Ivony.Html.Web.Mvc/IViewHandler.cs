using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 HTML 视图处理程序
  /// </summary>
  public interface IViewHandler : IViewDataContainer
  {

    /// <summary>
    /// 处理 HTML 范畴
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="scope">要处理的 HTML 范畴</param>
    /// <param name="urlHelper">有关视图的虚拟路径帮助器</param>
    void ProcessScope( ViewContext context, IHtmlContainer scope, JumonyUrlHelper urlHelper );
  }


  /// <summary>
  /// 需要提供自定义渲染代理的视图处理程序实现此接口
  /// </summary>
  public interface ICustomRenderAdapters : IViewHandler
  {
    /// <summary>
    /// 提供自定义渲染代理
    /// </summary>
    /// <returns>自定义的渲染代理</returns>
    IHtmlRenderAdapter[] GetCustomRenderAdapters();
  }

}
