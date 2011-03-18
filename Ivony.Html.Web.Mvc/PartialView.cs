using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public abstract class PartialView : JumonyViewBase
  {




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

      ProcessActions( Container );

      ProcessPartials( Container );

      //UNDONE Document.ResolveUriToAbsoluate();
    }

    protected abstract void ProcessContainer();





    protected virtual IHtmlContainer LoadContainer()
    {
      if ( VirtualPath == null )
        throw new InvalidOperationException();

      var document = HtmlProviders.LoadDocument( HttpContext, VirtualPath );

      return document.FindSingle( "body" );
    }

    protected override string RenderContent()
    {
      var builder = new StringBuilder();

      foreach ( var node in Container.Nodes() )
        builder.Append( node.Render() );


      return builder.ToString();
    }
  }
}
