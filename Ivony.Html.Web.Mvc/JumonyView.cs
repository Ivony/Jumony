using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using System.Web;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Web.Caching;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyView : IView, IHtmlHandler
  {


    public JumonyView( string virtualPath )
    {
      VirtualPath = virtualPath;
    }



    protected JumonyView()
    {

    }


    protected string VirtualPath
    {
      get;
      protected set;
    }


    protected IHtmlDocument Document
    {
      get;
      private set;
    }


    protected ViewContext ViewContext
    {
      get;
      private set;
    }

    protected object ViewModel
    {
      get { return ViewContext.ViewData.Model; }
    }

    protected ViewDataDictionary ViewData
    {
      get { return ViewContext.ViewData; }
    }

    protected HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
    }

    protected RequestContext RequestContext
    {
      get { return ViewContext.RequestContext; }
    }

    protected RouteData RouteData
    {
      get { return ViewContext.RouteData; }
    }

    protected TempDataDictionary TempData
    {
      get { return ViewContext.TempData; }
    }

    protected Cache Cache
    {
      get { return HttpContext.Cache; }
    }




    public void Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;
      Render( writer );

      var strings = new string[0];
      object[] objects = strings;
    }



    private MvcAdapter _adapter = new MvcAdapter();
    protected virtual MvcAdapter HtmlAdapter
    {
      get { return _adapter; }
    }



    protected virtual void Render( TextWriter output )
    {
      Document = LoadDocument( VirtualPath );

      ProcessDocument( ViewContext.HttpContext, Document );

      ProcessPartials();

      Document.ResolveUriToAbsoluate();

      var cacheKey = ViewContext.TempData["CacheKey"];

      if ( cacheKey != null )
      {
        var content = Document.Render( HtmlAdapter );
        UpdateCache( cacheKey, content );
      }
      else
      {
        Document.Render( output, HtmlAdapter );
      }

    }

    private void UpdateCache( object cacheKey, string content )
    {
      throw new NotImplementedException();
    }

    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      return HtmlProviders.LoadDocument( HttpContext, virtualPath );
    }


    public void ProcessDocument( HttpContextBase context, IHtmlDocument document )
    {
      return;
    }


    private void ProcessPartials()
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

        actionLink.SetAttribute( "href", href );
      }

    }



    private void AddGeneratorMetaData()
    {
      var modifier = Document.DomModifier;
      if ( modifier != null )
      {
        var header = Document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
        {

          var metaElement = modifier.AddElement( header, "meta" );

          metaElement.SetAttribute( "name", "generator" );
          metaElement.SetAttribute( "content", "Jumony" );
        }
      }
    }




    public void Dispose()
    {
    }
  }
}
