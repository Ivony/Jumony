<%@ WebHandler Language="C#" Class="Error" %>

using System;
using System.Web;
using Ivony.Html;
using Ivony.Html.Web;

public class Error : ViewHandler<Exception>
{


  protected override void ProcessScope()
  {
  }
}