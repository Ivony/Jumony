using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Web.Caching;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public abstract class ViewBase : IView, ICachableResult
  {

    protected string VirtualPath
    {
      get;
      set;
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


    protected UrlHelper Url
    {
      get;
      private set;
    }




    public virtual void Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;

      Url = new UrlHelper( viewContext.RequestContext );

      ProcessMain();

      var content = RenderContent();

      CachedResult = new ContentResult()
      {
        Content = content,
        ContentType = "text/html"
      };

      writer.Write( content );
    }


    protected abstract void ProcessMain();

    protected abstract string RenderContent();


    public ActionResult CachedResult
    {
      get;
      protected set;
    }




    protected void ProcessPartials( IHtmlContainer container )
    {

    }


    protected void ProcessActions( IHtmlContainer container )
    {

      var links = container.Find( "a[action]" );

      foreach ( var actionLink in links )
      {
        var action = actionLink.Attribute( "action" ).Value();
        var controller = actionLink.Attribute( "controller" ).Value();

        var routeValues = new RouteValueDictionary();

        foreach ( var attribute in actionLink.Attributes().Where( a => a.Name.StartsWith( "_" ) ).ToArray() )
        {
          routeValues.Add( attribute.Name.Substring( 1 ), attribute.Value() );
          attribute.Remove();
        }



        actionLink.Attribute( "action" ).Remove();

        var controllerAttribute = actionLink.Attribute( "controller" );
        if ( controllerAttribute != null )
          controllerAttribute.Remove();


        var href = Url.Action( action, controller, routeValues );

        actionLink.SetAttribute( "href", href );
      }


    }


    public virtual void Dispose()
    {
    }



  }
}
