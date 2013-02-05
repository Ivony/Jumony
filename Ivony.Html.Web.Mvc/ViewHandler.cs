using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Web.Routing;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// HTML 视图处理程序基类
  /// </summary>
  [Obsolete( "此类型已过时，仅出于兼容目的而保留，请使用 JumonyViewHandler 类型" )]
  public abstract class ViewHandler : JumonyView, IHttpHandler
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
    /// 获取页面文档对象
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 派生类重写此方法自定义文档处理逻辑
    /// </summary>
    protected abstract void ProcessDocument();


    /// <summary>
    /// 重写 Process 方法，调用 ProcessDocument 方法处理页面逻辑
    /// </summary>
    /// <param name="scope"></param>
    protected sealed override void ProcessScope( IHtmlContainer scope )
    {
      Document = scope.Document;
      ProcessDocument();
    }
  }


  /// <summary>
  /// 强类型 HTML 视图处理程序的基类
  /// </summary>
  /// <typeparam name="T">Model 的类型</typeparam>
  public abstract class ViewHandler<T> : ViewHandler
  {

    /// <summary>
    /// 模型
    /// </summary>
    protected new T ViewModel
    {
      get { return base.ViewModel.CastTo<T>(); }
    }

  }

}
