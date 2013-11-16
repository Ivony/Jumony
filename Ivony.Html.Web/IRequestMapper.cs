using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义请求映射器，Jumony请求映射器为请求获取 HTML 模板和对应的处理程序
  /// </summary>
  public interface IRequestMapper
  {

    /// <summary>
    /// 映射当前请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求</param>
    /// <returns>映射结果</returns>
    RequestMapping MapRequest( HttpRequestBase request );

  }

}
