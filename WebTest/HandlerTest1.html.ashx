<%@ WebHandler Language="C#" Class="HandlerTest1" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;


public class HandlerTest1 : HtmlHandler
{


  protected override void ProcessScope()
  {
    FindFirst( "body" ).InnerText( HtmlHandlerTest.testContent );
  }
}