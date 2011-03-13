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
      throw new NotSupportedException();
    }

    #endregion

    internal void Initialize( string virtualPath, bool isPartial )
    {
      VirtualPath = virtualPath;
      IsPartial = isPartial;
    }
  }


  public class ViewHandler<T> : JumonyView<T>, IHttpHandler
  {

    protected override void ProcessDocument()
    {
      ProcessDocument( ViewModel );
    }

    protected virtual void ProcessDocument( T model )
    {

    }


    #region IHttpHandler 成员

    public bool IsReusable
    {
      get { return false; }
    }

    public void ProcessRequest( HttpContext context )
    {
      throw new NotSupportedException();
    }

    #endregion
  }

}
