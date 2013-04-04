<%@ WebHandler Language="C#" Class="ChooseCssSelector_html" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;

public class ChooseCssSelector_html : ViewHandler
{
  protected override void ProcessDocument()
  {

    if ( ViewData["SelectedElements"] == null )
      Find( ".message" ).Remove();

  }
}