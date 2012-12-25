using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 扩展 UrlHelper 提供 Jumony 特有的方法。
  /// </summary>
  public class JumonyUrlHelper : UrlHelper
  {

    public JumonyUrlHelper( ViewBase view )
      : base( view.ViewContext.RequestContext )
    {
      ViewContext = view.ViewContext;
    }

    public ViewContext ViewContext
    {
      get;
      private set;
    }

    public string VirtualPath
    {
      get;
      private set;
    }

  }
}
