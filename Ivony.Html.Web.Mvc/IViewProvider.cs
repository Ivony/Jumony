using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public interface IViewProvider
  {

    PageView TryCreateView( ControllerContext context, string virtualPath, bool isPartial );

  }
}
