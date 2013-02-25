<%@ WebHandler Language="C#" Class="_filter" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;
using System.Linq;
using System.Web.Mvc;


public class _filter : ViewFilterHandler
{


  private static readonly ResourceManager resourceManager = new ResourceManager( "~/Views" );

  public override void OnPreRender( ViewContext context, JumonyView view )
  {
    var document = view.Scope as IHtmlDocument;
    if ( document != null )
      resourceManager.AddAllReference( document );
  }


}