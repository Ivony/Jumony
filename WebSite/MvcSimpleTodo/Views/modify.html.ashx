<%@ WebHandler Language="C#" Class="modify_html" %>

using System;
using System.Web;
using System.Linq;

using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Styles;
using Ivony.Html.Forms;
using Ivony.Html.Forms.Validation;
using Ivony.Html.Templates;


public class modify_html : ViewHandler<Task>
{

  protected override void ProcessDocument()
  {
    Document.FindSingle( "input[name=title]" ).SetAttribute( "value", ViewModel.Title );

  }

}