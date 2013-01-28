using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 母板视图控制程序的基类
  /// </summary>
  public abstract class MasterViewHandler : JumonyMasterView, IHttpHandler
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



  }
}
