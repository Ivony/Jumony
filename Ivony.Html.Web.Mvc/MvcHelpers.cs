using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public static class MvcHelpers
  {

    /// <summary>
    /// 获取为应用程序生成 URL 的帮助器
    /// </summary>
    /// <param name="requestContext">请求上下文</param>
    /// <param name="virtualPath">视图 URL 地址</param>
    /// <returns>URL 帮助器</returns>
    public static JumonyUrlHelper GetUrlHelper( RequestContext requestContext, string virtualPath )
    {
      return new JumonyUrlHelper( requestContext, virtualPath );
    }
  }
}
