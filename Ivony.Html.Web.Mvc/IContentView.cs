using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 可使用母板页的内容视图
  /// </summary>
  public interface IContentView : IView
  {

    /// <summary>
    /// 初始化母板
    /// </summary>
    /// <param name="master">母板视图</param>
    void InitializeMaster( IMasterView master );

    /// <summary>
    /// 创建内容渲染代理
    /// </summary>
    /// <param name="master">所使用的母板视图</param>
    /// <returns>用于渲染内容视图的渲染代理</returns>
    IHtmlRenderAdapter CreateContentAdapter( IMasterView master );
  }
}
