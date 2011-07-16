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

      ProcessActionLinks( Container );

      if ( VirtualPath != null )//若不是内嵌部分视图，则应当转换URL。
        ResolveUri( Container, Container.Document.DocumentUri );
    }

    protected abstract void ProcessContainer();




    protected virtual IHtmlContainer LoadContainer()
    {
      var document = HtmlProviders.LoadDocument( HttpContext, VirtualPath );

      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }

    protected override string RenderContent()
    {
      var writer = new StringWriter();


      foreach ( var node in Container.Nodes() )
        node.Render( writer, RenderAdapters.ToArray() );


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
