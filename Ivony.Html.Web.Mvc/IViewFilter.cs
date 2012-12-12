using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 定义视图筛选器
  /// </summary>
  public interface IViewFilter
  {

    /// <summary>
    /// 在处理 HTML 文档之前由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    void OnPreProcess( ViewContext context, ViewBase view );
    /// <summary>
    /// 在处理 HTML 文档之后由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    void OnPostProcess( ViewContext context, ViewBase view );
    /// <summary>
    /// 在渲染 HTML 文档之前由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    void OnPreRender( ViewContext context, ViewBase view );
    /// <summary>
    /// 在渲染 HTML 文档之后由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    void OnPostRender( ViewContext context, ViewBase view );

  }





  /// <summary>
  /// 表示所有视图筛选器特性的基类
  /// </summary>
  public abstract class ViewFilterAttribute : ActionFilterAttribute, IViewFilter
  {
    /// <summary>
    /// 重写 OnActionExecuting 方法，不进行任何操作
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public sealed override void OnActionExecuting( ActionExecutingContext filterContext )
    {
    }

    /// <summary>
    /// 重写 OnActionExecuted 方法，不进行任何操作
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public sealed override void OnActionExecuted( ActionExecutedContext filterContext )
    {
    }

    /// <summary>
    /// 重写 OnResultExecuting 方法，在 ViewData 中注册视图筛选器
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public sealed override void OnResultExecuting( ResultExecutingContext filterContext )
    {

      var viewResult = filterContext.Result as ViewResultBase;
      if ( viewResult == null )
        return;

      var filters = viewResult.ViewData[ViewBase.ViewFiltersDataKey] as IList<IViewFilter>;
      if ( filters != null )
        filters.Add( this );

      else
        viewResult.ViewData[ViewBase.ViewFiltersDataKey] = new List<IViewFilter>( new[] { this } );

    }

    /// <summary>
    /// 重写 OnResultExecuted 方法，不进行任何操作
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public sealed override void OnResultExecuted( ResultExecutedContext filterContext )
    {
    }

    /// <summary>
    /// 在处理 HTML 文档之前由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    public virtual void OnPreProcess( ViewContext context, ViewBase view )
    {
    }

    /// <summary>
    /// 在处理 HTML 文档之后由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    public virtual void OnPostProcess( ViewContext context, ViewBase view )
    {
    }

    /// <summary>
    /// 在渲染 HTML 文档之前由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    public virtual void OnPreRender( ViewContext context, ViewBase view )
    {
    }

    /// <summary>
    /// 在渲染 HTML 文档之后由 Jumony 框架调用
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="view">正在负责处理的视图对象</param>
    public virtual void OnPostRender( ViewContext context, ViewBase view )
    {
    }
  }
}
