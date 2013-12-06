using Ivony.Html.Web;
using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

/// <summary>
/// TestJumonyHandler 的摘要说明
/// </summary>
public class TestJumonyHandler : JumonyHandler
{

  public ICachedResponse ProcessRequest( HttpContextBase context, string virtualPath, bool isPartial = false )
  {
    return base.ProcessRequest( context, virtualPath, isPartial );
  }

}