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


    /// <summary>
    /// 为现有的 IHtmlHandler 对象创建 ViewHandler 包装
    /// </summary>
    /// <param name="handler">现有的 IHtmlHandler 对象</param>
    public HtmlViewHandlerWrapper( IHtmlHandler handler )
    {
      _handler = handler;
    }



    /// <summary>
    /// 重写 ProcessScope 方法，重定向到被包装的 Handler
    /// </summary>
    protected override void ProcessScope()
    {
      _handler.ProcessScope( new HtmlRequestContext( HttpContext, VirtualPath, Scope ) );
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
