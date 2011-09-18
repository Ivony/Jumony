<%@ WebHandler Language="C#" Class="Test" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Web.Mvc;
using System.Diagnostics;

public class Test : PartialViewHandler
{


  protected override void ProcessContainer()
  {
    int i = 0;
    while ( true ) Debug.WriteLine( unchecked( i++ ) );
  }
}