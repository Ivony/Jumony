using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 处理程序提供程序
  /// </summary>
  public interface IHtmlHandlerProvider
  {

    /// <summary>
    /// 获取 HTML 处理程序
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>需要处理该 HTML 文档的 HTML 处理程序</returns>
    IHtmlHandler GetHandler( string virtualPath );
  }
}
