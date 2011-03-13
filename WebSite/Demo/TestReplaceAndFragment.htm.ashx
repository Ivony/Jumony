<%@ WebHandler Language="C#" Class="TestReplaceAndFragment" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Web;

public class TestReplaceAndFragment : JumonyHandler
{
  protected override void ProcessDocument()
  {
    var body = Find( "body" ).Single();

    var fragment = Document.ParseFragment( "<body />" );

    body.ReplaceWith( fragment );
  }

}