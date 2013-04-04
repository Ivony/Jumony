using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public class JumonyViewHandler : JumonyView, IHttpHandler
  {

    #region IHttpHandler 成员

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      throw new HttpException( 404, "不能直接访问视图处理程序" );
    }

    #endregion


    /// <summary>
    /// 在处理范畴内查找符合选择器的元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的元素</returns>
    protected IEnumerable<IHtmlElement> Find( string expression )
    {
      return Scope.Find( expression );
    }


    /// <summary>
    /// 在处理范畴内查找符合选择器的唯一元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的唯一元素</returns>
    protected IHtmlElement FindSingle( string expression )
    {
      return Scope.FindSingle( expression );
    }

    /// <summary>
    /// 在处理范畴内查找符合选择器的首个元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的首个元素</returns>
    protected IHtmlElement FindFirst( string expression )
    {
      return Scope.FindFirst( expression );
    }

    /// <summary>
    /// 在处理范畴内查找符合选择器的最后一个元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的最后一个元素</returns>
    protected IHtmlElement FindLast( string expression )
    {
      return Scope.FindLast( expression );
    }



    /// <summary>
    /// 对处理范畴内查找符合选择器的首个元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForFirst( string expression, Action<IHtmlElement> action )
    {
      Scope.Find( expression ).ForFirst( action );
    }

    /// <summary>
    /// 对处理范畴内查找符合选择器的唯一元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForSingle( string expression, Action<IHtmlElement> action )
    {
      Scope.Find( expression ).ForSingle( action );
    }

    /// <summary>
    /// 对处理范畴内查找符合选择器的最后一个元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForLast( string expression, Action<IHtmlElement> action )
    {
      Scope.Find( expression ).ForLast( action );
    }


    /// <summary>
    /// 对处理范畴内查找符合选择器的所有元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForAll( string expression, Action<IHtmlElement> action )
    {
      Scope.Find( expression ).ForAll( action );
    }


    /// <summary>
    /// 获取视图模型
    /// </summary>
    protected object ViewModel
    {
      get { return ViewContext.ViewData.Model; }
    }

    /// <summary>
    /// 获取视图数据
    /// </summary>
    protected ViewDataDictionary ViewData
    {
      get { return ViewContext.ViewData; }
    }

    /// <summary>
    /// 获取当前 HTTP 响应的追踪上下文对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }



    /// <summary>
    /// 获取请求上下文
    /// </summary>
    protected RequestContext RequestContext
    {
      get { return ViewContext.RequestContext; }
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    protected RouteData RouteData
    {
      get { return ViewContext.RouteData; }
    }

    /// <summary>
    /// 获取 TempData
    /// </summary>
    protected TempDataDictionary TempData
    {
      get { return ViewContext.TempData; }
    }

    /// <summary>
    /// 获取缓存提供对象
    /// </summary>
    protected Cache Cache
    {
      get { return HttpContext.Cache; }
    }


  }
}
