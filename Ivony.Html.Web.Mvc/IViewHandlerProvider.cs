using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 ViewHandler 对象提供程序
  /// </summary>
  public interface IViewHandlerProvider
  {

    /// <summary>
    /// 查找 ViewHandler 对象
    /// </summary>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <returns>要用于处理视图的 ViewHandler 对象</returns>
    IViewHandler FindViewHandler( string virtualPath );

  }
}
