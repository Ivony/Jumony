using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 请求上下文
  /// </summary>
  public class HtmlRequestContext
  {


    public HtmlRequestContext( HttpContextBase httpContext, string virtualPath )
    {

      if ( httpContext == null )
        throw new ArgumentNullException( "httpContext" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      HttpContext = httpContext;
      VirtualPath = virtualPath;
    }

    /// <summary>
    /// 当前 HTTP 请求
    /// </summary>
    public HttpContextBase HttpContext { get; private set; }


    /// <summary>
    /// 当前处理的文档的虚拟路径
    /// </summary>
    public string VirtualPath { get; private set; }

  }
}
