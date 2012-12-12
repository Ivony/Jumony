<%@ WebHandler Language="C#" Class="iThrough" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using Ivony.Html.Web;
using Ivony.Html.Styles;
using Ivony.Html.Forms;

public class iThrough : IHttpHandler
{

  public void ProcessRequest( HttpContext context )
  {

    var parser = new JumonyParser();

    var dataDocument = parser.LoadDocument( "http://www.360buy.com" );
    var templateDocument = parser.LoadDocument( "http://www.ithrough.com/mac-accessories" );

    
    templateDocument.ResolveUriToAbsoluate();
    dataDocument.ResolveUriToAbsoluate();

    dataDocument.Find( "ul[class=list-h] li" ).BindTo( templateDocument.Find( ".items > .itembox" ), ( dataElement, targetElement ) =>
    {
      string name = dataElement.Find( "[class=p-name]" ).Single().InnerText();
      string price = dataElement.Find( "[class=p-price]" ).Single().InnerText();
      string image = dataElement.Find( "[class=p-img] img" ).Single().Attribute( "src2" ).Value();
      string link = dataElement.Find( "[class=p-img] a" ).First().Attribute( "href" ).Value();

      targetElement.Find( "a" ).SetAttribute( "href", link ).SetAttribute( "target", "_blank" );
      targetElement.Find( ".proPri" ).Single().InnerText( price );
      targetElement.Find( ".proName" ).Single().InnerText( name );
      targetElement.Find( "a img" ).Single().SetAttribute( "src", image );
    } );


    templateDocument.Render( context.Response.Output );
    
  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

}