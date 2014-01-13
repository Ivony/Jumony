<%@ WebHandler Language="C#" Class="Test2" %>

using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Web;

using System;
using System.Web;


public class Test2 : HtmlHandler
{

  protected override void ProcessGet()
  {
    FindFirst( "body" ).AddElement( "partial" ).SetAttribute( "path", "~/PartialTest/Partial1.html" );
  }

}