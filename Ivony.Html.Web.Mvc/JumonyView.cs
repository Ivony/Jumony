using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyView : IView
  {

    protected string VirtualPath
    {
      get;
      private set;
    }


    protected IHtmlDocument Document
    {
      get;
      private set;
    }


    public JumonyView( string virtualPath, IHtmlDocument document )
    {
      VirtualPath = virtualPath;
      Document = document;
    }



    public void Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;
      Render( writer );
    }


    public ViewContext ViewContext
    {
      get;
      private set;
    }


    protected virtual void Render( TextWriter output )
    {

      ProcessDocument();

      ProcessActionLinks();



      AddGeneratorMetaData();

      output.Write( Document.InnerHtml( true ) );
    }


    protected virtual void ProcessDocument()
    {
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

        actionLink.SetAttribute( "href" ).Value( href );
      }

    }



    private void AddGeneratorMetaData()
    {
      var factory = Document.GetNodeFactory();
      if ( factory != null )
      {
        var meta = factory.CreateElement( "meta" );
        meta.SetAttribute( "name" ).Value( "generator" );
        meta.SetAttribute( "content" ).Value( "Jumony" );

        var header = Document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
          meta.InsertTo( header, 0 );

      }
    }


  }
}
