<%@ WebHandler Language="C#" Class="_filter" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;
using System.Linq;
using System.Web.Mvc;


public class _filter : IHttpHandler, IViewFilter
{


  private static readonly ResourceManager resourceManager = new ResourceManager( "~/Views" );

  public void OnPreRender( ViewContext context, JumonyView view )
  {
    var document = view.Scope as IHtmlDocument;
    if ( document != null )
      resourceManager.AddAllReference( document );
  }



  bool IHttpHandler.IsReusable
  {
    get { return true; }
  }

  void IHttpHandler.ProcessRequest( HttpContext context )
  {
    throw new NotSupportedException();
  }

  public void OnPreProcess( ViewContext context, JumonyView view )
  {
  }

  public void OnPostProcess( ViewContext context, JumonyView view )
  {
  }

  public void OnPostRender( ViewContext context, JumonyView view )
  {
  }
}