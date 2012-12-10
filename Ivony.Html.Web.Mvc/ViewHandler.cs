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
  public abstract class ViewHandler : PageView, IHttpHandler
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



    internal void Initialize( string virtualPath )
    {
      VirtualPath = virtualPath;
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
