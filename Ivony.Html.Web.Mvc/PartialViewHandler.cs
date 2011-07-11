using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// 部分视图处理程序抽象基类
  /// </summary>
  public abstract class PartialViewHandler : PartialView, IHttpHandler
  {
    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      throw new HttpException( 404, "不能直接访问部分视图处理程序" );
    }



    internal void Initialize( string virtualPath )
    {
      VirtualPath = virtualPath;
    }



  }

  public abstract class PartialViewHandler<T> : PartialViewHandler
  {

    protected new T ViewModel
    {
      get { return base.ViewModel.CastTo<T>(); }
    }

  }
}
