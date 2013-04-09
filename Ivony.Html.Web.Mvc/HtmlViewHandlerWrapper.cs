using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  internal class HtmlViewHandlerWrapper : IViewHandler, IHandlerWrapper, IDisposable
  {
    private IHtmlHandler _handler;

    public HtmlViewHandlerWrapper( IHtmlHandler handler )
    {
      _handler = handler;
    }


    public void ProcessScope( ViewContext context, IHtmlContainer scope, JumonyUrlHelper urlHelper )
    {
      _handler.ProcessDocument( context.HttpContext, scope.Document );
    }

    public ViewDataDictionary ViewData
    {
      get;
      set;
    }

    void IDisposable.Dispose()
    {
      _handler.Dispose();
    }


    object IHandlerWrapper.Handler
    {
      get { return _handler; }
    }
  }
}
