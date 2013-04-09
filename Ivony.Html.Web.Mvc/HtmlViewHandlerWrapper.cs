using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  internal class HtmlViewHandlerWrapper : ViewHandler, IHandlerWrapper, IDisposable
  {
    private IHtmlHandler _handler;

    public HtmlViewHandlerWrapper( IHtmlHandler handler )
    {
      _handler = handler;
    }



    protected override void ProcessScope()
    {
      _handler.ProcessDocument( HttpContext, Scope.Document );
    }


    void IDisposable.Dispose()
    {
      var disposable = _handler as IDisposable;
      if ( disposable != null )
        _handler.Dispose();
    }


    object IHandlerWrapper.Handler
    {
      get { return _handler; }
    }
  }
}
