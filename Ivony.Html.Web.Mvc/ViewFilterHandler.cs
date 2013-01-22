using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web.Mvc
{
  public class ViewFilterHandler : IHttpHandler, IViewFilter
  {
    public bool IsReusable
    {
      get { return true; }
    }

    public void ProcessRequest( HttpContext context )
    {
      throw new HttpException( 404, "不能直接访问视图筛选处理器" );
    }



    public virtual void OnPreProcess( System.Web.Mvc.ViewContext context, JumonyView view )
    {
    }

    public virtual void OnPostProcess( System.Web.Mvc.ViewContext context, JumonyView view )
    {
    }

    public virtual void OnPreRender( System.Web.Mvc.ViewContext context, JumonyView view )
    {
    }

    public virtual void OnPostRender( System.Web.Mvc.ViewContext context, JumonyView view )
    {
    }
  }
}
