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
  public class ViewHandler : PageView, IHttpHandler
  {


    protected override void ProcessDocument()
    {

      ProcessDocument( ViewModel );

    }

    protected virtual void ProcessDocument( dynamic model )
    {

    }




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


  public class ViewHandler<T> : ViewHandler
  {

    protected new T ViewModel
    {
      get { return base.ViewModel.CastTo<T>(); }
    }

  }

}
