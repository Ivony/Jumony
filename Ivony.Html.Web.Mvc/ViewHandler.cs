using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  public class ViewHandler : JumonyView, IHttpHandler
  {





    #region IHttpHandler 成员

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      throw new NotSupportedException();
    }

    #endregion

    internal void SetVirtualPath( string virtualPath )
    {
      VirtualPath = virtualPath;
    }
  }
}
