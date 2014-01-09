<%@ WebHandler Language="C#" Class="Partial2" %>

using System;
using System.Web;
using Ivony.Html.Web;
using Ivony.Html;

public class Partial2 : HtmlHandler
{

  protected override void ProcessGet()
  {
    FindFirst( "#container" ).InnerText( PartialTest.testContent );
  }


}