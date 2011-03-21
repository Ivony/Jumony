using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public abstract class PartialView : ViewBase
  {


    protected PartialView()
    {

    }

    protected PartialView( string virtualPath )
    {
      VirtualPath = virtualPath;
    }



    protected IHtmlContainer Container
    {
      get;
      private set;
    }

    protected override void ProcessMain()
    {
      if ( Container == null )
        Container = LoadContainer();

      ProcessContainer();

      ProcessLinks( Container );

      ProcessPartials( Container );


      if ( VirtualPath != null )
        Container.Document.ResolveUriToAbsoluate();
    }

    protected abstract void ProcessContainer();




    internal void Initialize( string virtualPath )
    {
      VirtualPath = virtualPath;
    }


    internal void Initialize( IHtmlContainer container )
    {
      Container = container;
    }




    protected virtual IHtmlContainer LoadContainer()
    {
      if ( VirtualPath == null )
      {
        throw new InvalidOperationException();
      }

      var document = HtmlProviders.LoadDocument( HttpContext, VirtualPath );

      var body =  document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }

    protected override string RenderContent()
    {
      var writer = new StringWriter();

      foreach ( var node in Container.Nodes() )
        node.Render( writer, RenderAdapter );


      return writer.ToString();
    }
  }


  public class GenericPartialView : PartialView
  {

    public GenericPartialView( string virtualPath )
      : base( virtualPath )
    { }




    protected override void ProcessContainer()
    {
      return;
    }
  }



}
