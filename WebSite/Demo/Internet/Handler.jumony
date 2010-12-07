<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Net;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using System.Linq;
using Ivony.Html.HtmlAgilityPackAdaptor;

public class Handler : IHttpHandler
{

  public void ProcessRequest( HttpContext context )
  {
    context.Response.ContentType = "text/html";

    var client = new WebClient();

    var parser = new JumonyHtmlParser();

    var dataDocument = parser.Parse( client.DownloadString( "http://www.360buy.com" ) );
    var templateDocument = parser.Parse( client.DownloadString( "http://www.wsdeal.com/" ) );


    dataDocument.Find( "ul[class=list-h] li" ).BindTo( templateDocument.Find( "div.index_3_b1 ul li" ), ( dataElement, targetElement ) =>
      {
        string name = dataElement.Find( "[class=p-name]" ).Single().InnerText();
        string price = dataElement.Find( "[class=p-price]" ).Single().InnerText();
        string image = dataElement.Find( "[class=p-img] img" ).Single().Attribute( "src2" ).Value();
        string link = dataElement.Find( "[class=p-img] a" ).First().Attribute( "href" ).Value();

        targetElement.Find( "a" ).SetAttribute( "href" ).Value( link ).SetAttribute( "target" ).Value( "_blank" );
        targetElement.Find( "div.index_3_top1_txt3" ).Single().InnerText( price );
        targetElement.Find( "div.index_3_top1_txt1a a" ).Single().InnerText( name );
        targetElement.Find( "a img" ).Single().SetAttribute( "src" ).Value( image );
      } );


    var baseElement = templateDocument.GetNodeFactory().CreateElement( "base" );
    baseElement.SetAttribute( "href" ).Value( "http://www.wsdeal.com/" );

    templateDocument.Find( "head" ).First().Insert( 0, baseElement );

    context.Response.Write( templateDocument.InnerHtml( false ) );


  }

  public bool IsReusable
  {
    get
    {
      return false;
    }
  }

}