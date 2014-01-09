<%@ WebHandler Language="C#" Class="Test1" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;


public class Test1 : HtmlHandler
{


  protected override void ProcessGet()
  {
    FindFirst( "body" ).InnerText( HtmlHandlerTest.testContent );
  }
}