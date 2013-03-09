using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 HTML 视图处理程序
  /// </summary>
  public interface IHtmlViewHandler : IViewDataContainer, IDisposable
  {

    /// <summary>
    /// 处理 HTML 范畴
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="scope">要处理的 HTML 范畴</param>
    void ProcessScope( ViewContext context, IHtmlContainer scope );

  }


  internal class HtmlViewHandlerWrapper : IHtmlViewHandler
  {
    private IHtmlHandler _handler;

    public HtmlViewHandlerWrapper( IHtmlHandler handler )
    {
      _handler = handler;
    }


    public void ProcessScope( ViewContext context, IHtmlContainer scope )
    {
      _handler.ProcessDocument( context.HttpContext, scope.Document );
    }

    public ViewDataDictionary ViewData
    {
      get;
      set;
    }

    public void Dispose()
    {
      _handler.Dispose();
    }
  }


}
