using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 路径依赖视图筛选器的基类，通过在指定虚拟路径放置_filter.ashx文件，并继承此类型实现路径依赖的视图筛选器
  /// </summary>
  public abstract class ViewFilterHandler : IHttpHandler, IViewFilter
  {


    #region IHttpHandler 成员

    /// <summary>
    /// 重写 IsReusable 属性，始终返回 true。
    /// </summary>
    public virtual bool IsReusable
    {
      get { return true; }
    }

    /// <summary>
    /// 重写 ProcessRequest 方法，始终抛出 NotFound 异常。
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest( HttpContext context )
    {
      throw new HttpException( 404, "不能直接访问视图筛选处理器" );
    }

    #endregion



    /// <summary>
    /// 当视图被处理前执行
    /// </summary>
    /// <param name="context">当前视图上下文</param>
    /// <param name="view">正在处理的视图</param>
    public virtual void OnPreProcess( ViewContext context, JumonyView view )
    {
    }


    /// <summary>
    /// 当视图被处理后执行
    /// </summary>
    /// <param name="context">当前视图上下文</param>
    /// <param name="view">正在处理的视图</param>
    public virtual void OnPostProcess( ViewContext context, JumonyView view )
    {
    }


    /// <summary>
    /// 当视图被渲染前执行
    /// </summary>
    /// <param name="context">当前视图上下文</param>
    /// <param name="view">正在处理的视图</param>
    public virtual void OnPreRender( ViewContext context, JumonyView view )
    {
    }


    /// <summary>
    /// 当视图被渲染后执行
    /// </summary>
    /// <param name="context">当前视图上下文</param>
    /// <param name="view">正在处理的视图</param>
    public virtual void OnPostRender( ViewContext context, JumonyView view )
    {
    }
  }
}
