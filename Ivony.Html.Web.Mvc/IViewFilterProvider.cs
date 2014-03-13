using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义视图筛选器提供程序
  /// </summary>
  public interface IViewFilterProvider
  {

    /// <summary>
    /// 获取指定虚拟路径所使用的视图筛选器
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>视图筛选器</returns>
    IViewFilter[] GetFilters( string virtualPath );

  }
}
