<%@ WebHandler Language="C#" Class="_filter" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Web.Mvc;
using System.Linq;
using System.Web.Mvc;


public class _filter : ViewFilterHandler
{

  public override void OnPreRender( ViewContext context, JumonyView view )
  {
    view.Scope.Find( "title" ).First().InnerHtml( "Test" );
  }


}