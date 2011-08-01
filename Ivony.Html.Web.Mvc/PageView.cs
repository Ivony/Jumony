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
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  public abstract class PageView : ViewBase
  {
    public PageView( string virtualPath )
    {
      VirtualPath = virtualPath;
    }



    protected PageView()
    {

    }




    protected IHtmlDocument Document
    {
      get;
      private set;
    }


    protected override string RenderContent()
    {
      return Document.Render( RenderAdapters.ToArray() );
    }


    protected override void ProcessMain()
    {
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin LoadDocument" );
      Document = LoadDocument();
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End LoadDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessDocument" );
      ProcessDocument();
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessActionLinks" );
      ProcessActionLinks( Document );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessActionLinks" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ResolveUri" );
      ResolveUri( Document );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ResolveUri" );


      AddGeneratorMetaData();

    }

    protected virtual IHtmlDocument LoadDocument()
    {
      return HtmlProviders.LoadDocument( HttpContext, VirtualPath );
    }


    protected abstract void ProcessDocument();



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


  }



  internal class GenericPageView : PageView
  {

    public GenericPageView( string virtualPath )
      : base( virtualPath )
    {

    }


    protected override void ProcessDocument()
    {
      return;
    }
  }
}
