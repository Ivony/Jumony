<%@ WebHandler Language="C#" Class="BindingSheet" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Web;

public class BindingSheet : JumonyHandler
{
  protected override void ProcessDocument()
  {

    var a = new jQuery( "#dataList", Document );
    
  }
}
