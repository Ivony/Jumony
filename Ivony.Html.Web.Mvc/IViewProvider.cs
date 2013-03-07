using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Hosting;

namespace Ivony.Html.Web
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
    /// <param name="provider">用于获取虚拟文件的虚拟路径提供程序</param>
    /// <param name="virtualPath">用于查找视图的虚拟路径</param>
    /// <param name="isPartial">是否应创建部分视图</param>
    /// <returns>自定义视图对象</returns>
    ViewBase TryCreateView( ControllerContext context, VirtualPathProvider provider, string virtualPath, bool isPartial );

  }
}
