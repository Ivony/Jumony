using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 处理程序基类，协助实现 HTML 处理程序
  /// </summary>
  public abstract class HtmlHandler : HtmlHandlerBase, IHtmlHandler, IHttpHandler
  {


    /// <summary>
    /// 处理 HTML 范围
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    /// <param name="scope">HTML 范围</param>
    public void ProcessScope( HtmlRequestContext context, IHtmlContainer scope )
    {
      Context = context;
      Scope = scope;

      ProcessScope();
    }

    protected abstract void ProcessScope();





    protected IHtmlContainer Scope
    {
      get;
      private set;
    }

    protected override IHtmlContainer HtmlScope
    {
      get { return Scope; }
    }



    protected HtmlRequestContext Context
    {
      get;
      private set;
    }



    protected override System.Web.HttpContextBase HttpContext
    {
      get { return Context.HttpContext; }
    }

    public virtual void Dispose()
    {
    }

    public bool IsReusable
    {
      get { throw new NotImplementedException(); }
    }

    public void ProcessRequest( HttpContext context )
    {
      throw JumonyHandler.DirectVisitError();
    }
  }
}
