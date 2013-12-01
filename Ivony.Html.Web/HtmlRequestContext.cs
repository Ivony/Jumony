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
      HttpContext = httpContext;
      VirtualPath = virtualPath;
    }

    public HttpContextBase HttpContext { get; private set; }

    public string VirtualPath { get; private set; }

  }
}
