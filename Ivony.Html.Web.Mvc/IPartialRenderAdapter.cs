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
    void RenderPartial( IHtmlElement partialElement, ViewContext context, TextWriter writer );
  }
}
