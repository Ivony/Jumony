using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  
  /// <summary>
  /// 自定义视图提供程序
  /// </summary>
  public interface IViewProvider
  {

    PageView TryCreateView( ControllerContext context, string virtualPath, bool isPartial );

  }
}
