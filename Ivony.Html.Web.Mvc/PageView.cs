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
      return Document.Render( RenderAdapter );
    }


    protected override void ProcessMain()
    {
      Document = LoadDocument();

      ProcessDocument();

      ProcessActions( Document );

      ProcessPartials( Document );

      AddGeneratorMetaData();


      Document.ResolveUriToAbsoluate();
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
