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
  public class HtmlHandler : HtmlHandlerBase, IHtmlHandler, IHttpHandler
  {


    /// <summary>
    /// 处理 HTML 范围
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    public void ProcessScope( HtmlRequestContext context )
    {
      Context = context;

      ProcessScope();
    }



    /// <summary>
    /// 派生类实现此方法对 HTML 范围进行处理
    /// </summary>
    protected virtual void ProcessScope() { }




    /// <summary>
    /// 要进行处理的 HTML 范围
    /// </summary>
    protected override IHtmlContainer HtmlScope
    {
      get { return Context.Scope; }
    }



    /// <summary>
    /// 当前 HTML 请求上下文
    /// </summary>
    protected HtmlRequestContext Context
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前 HTTP 请求上下文
    /// </summary>
    protected override HttpContextBase HttpContext
    {
      get { return Context.HttpContext; }
    }


    /// <summary>
    /// 此方法用于实现销毁对象的一些操作。
    /// </summary>
    public virtual void Dispose()
    {
    }



    /// <summary>
    /// 此属性指示该处理器是否可以重用，其始终返回 false。
    /// </summary>
    public bool IsReusable
    {
      get { return false; }
    }


    /// <summary>
    /// 实现此方法当使用此处理器直接处理 HTTP 请求时，直接抛出异常。
    /// </summary>
    /// <param name="context">HTTP 请求上下文</param>
    public void ProcessRequest( HttpContext context )
    {
      throw JumonyHandler.DirectVisitError();
    }
  }
}
