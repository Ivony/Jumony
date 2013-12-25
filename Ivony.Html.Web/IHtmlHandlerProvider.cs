using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 处理器提供程序
  /// </summary>
  public interface IHtmlHandlerProvider
  {

    /// <summary>
    /// 获取 HTML 处理器
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>需要处理该 HTML 文档的 HTML 处理器</returns>
    IHtmlHandler GetHandler( string virtualPath );
  }
}
