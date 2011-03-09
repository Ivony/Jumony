using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  public abstract class ViewHandler : JumonyHandler, IView
  {

    protected ViewContext ViewContext
    {
      get;
      private set;
    }

    protected TextWriter Writer
    {
      get;
      private set;
    }


    public void Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;
      Writer = writer;

      ProcessRequestCore( ViewContext.HttpContext );
    }

    protected override void OnPostProcessDocument()
    {
      
      base.OnPostProcessDocument();

      ProcessActionLinks();

      Document.ResolveUriToAbsoluate();
    
    }

    protected void ProcessActionLinks()
    {

      var links = Document.Find( "a[action]" );

      foreach ( var actionLink in links )
      {
        var action = actionLink.Attribute( "action" ).Value();
        var controller = actionLink.Attribute( "controller" ).Value();

        var routeValues = new RouteValueDictionary();

        foreach ( var attribute in actionLink.Attributes().Where( a => a.Name.StartsWith( "_" ) ) )
        {
          routeValues.Add( attribute.Name.Substring( 1 ), attribute.Value() );
          attribute.Remove();
        }



        actionLink.Attribute( "action" ).Remove();

        var controllerAttribute = actionLink.Attribute( "controller" );
        if ( controllerAttribute != null )
          controllerAttribute.Remove();


        var href = UrlHelper.GenerateUrl( null, action, controller, routeValues, RouteTable.Routes, ViewContext.RequestContext, true );

        actionLink.SetAttribute( "href", href );
      }

    }


    protected override void OutputResponse( RawResponse responseData )
    {
      Writer.Write( responseData.Content );
    }

  }
}
