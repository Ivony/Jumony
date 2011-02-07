using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public interface IViewProvider
  {

    JumonyView TryCreateView( string virtualPath, WebPage page );

  }
}
