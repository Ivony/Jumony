using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  
  /// <summary>
  /// 定义一个部分视图的渲染代理
  /// </summary>
  public interface IPartialRenderAdapter
  {
    /// <summary>
    /// 渲染一个部分视图
    /// </summary>
    /// <param name="partialElement">部分视图标签</param>
    /// <param name="context">当前视图上下文</param>
    /// <param name="writer">用于输出部分视图的编写器</param>
    string RenderPartial( IHtmlElement partialElement, ViewContext context );
  }
}
