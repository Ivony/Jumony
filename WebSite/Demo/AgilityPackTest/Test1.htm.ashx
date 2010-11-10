<%@ WebHandler Language="C#" Class="Test1" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Fluent;
using Ivony;
using Ivony.Html;
using Ivony.Html.Forms;
using Ivony.Html.HtmlAgilityPackAdaptor;

public class Test1 : Ivony.Html.Web.JumonyHandler
{


  protected override void ProcessDocument()
  {
    throw new NotImplementedException();
  }

  protected override IHtmlDocument ParseDocument( string documentContent )
  {
    throw new NotImplementedException();
  }
}