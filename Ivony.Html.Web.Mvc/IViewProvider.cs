using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 自定义视图提供程序
  /// </summary>
  public interface IViewProvider
  {

    /// <summary>
    /// 尝试创建自定义视图对象
    /// </summary>
    /// <param name="context">上下文信息</param>
    /// <param name="virtualPath">用于查找视图的虚拟路径</param>
    /// <returns>自定义视图对象</returns>
    ViewBase TryCreateView( ControllerContext context, string virtualPath, bool isPartial );

  }
}
