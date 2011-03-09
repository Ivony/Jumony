using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public class MvcMapResult : RequestMapResult
  {

    public ViewContext ViewContext
    {
      get;
      private set;
    }

    public MvcMapResult( ViewContext context, string virtualPath, IHtmlHandler handler )
      : base( virtualPath, handler )
    {
      ViewContext = context;
    }
  }
}
