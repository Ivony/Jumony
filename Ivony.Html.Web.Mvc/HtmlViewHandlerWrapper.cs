using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  internal class HtmlViewHandlerWrapper : IViewHandler, IDisposable
  {
    internal IHtmlHandler Handler
    {
      get;
      private set;
    }

    public HtmlViewHandlerWrapper( IHtmlHandler handler )
    {
      Handler = handler;
    }


    public void ProcessScope( ViewContext context, IHtmlContainer scope, JumonyUrlHelper urlHelper )
    {
      Handler.ProcessDocument( context.HttpContext, scope.Document );
    }

    public ViewDataDictionary ViewData
    {
      get;
      set;
    }

    void IDisposable.Dispose()
    {
      Handler.Dispose();
    }
  }
}
