using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{

  public class ContentAdapter : IHtmlAdapter
  {


    protected JumonyView View { get; private set; }

    protected IHtmlDocument Document
    {
      get { return (IHtmlDocument) View.Scope; }
    }

    public ContentAdapter( JumonyView view )
    {
      if ( view.PartialMode )
        throw new InvalidOperationException( "部分视图不能套用母板" );

      View = view;
    }

    bool IHtmlAdapter.Render( IHtmlNode node, TextWriter writer )
    {
      var element = node as IHtmlElement;

      if ( element == null )
        return false;

      if ( element.Name.EqualsIgnoreCase( "content" ) )
      {
        var body = Document.FindSingle( "body" );
        var contentBodyId = body.Attribute( "content-body" ).Value();

        if ( !string.IsNullOrEmpty( contentBodyId ) )
          body = Document.GetElementById( contentBodyId );


        foreach ( var contentNode in body.Nodes() )
          contentNode.Render( writer );

        return true;
      }
      else if ( element.Name.EqualsIgnoreCase( "head" ) )
      {

        View.ViewContext.HttpContext.Trace.Write( "ContentView", "Begin Merge Head" );
        var head = MergeHead( element, Document.FindSingle( "head" ) );
        View.ViewContext.HttpContext.Trace.Write( "ContentView", "End Merge Head" );

        head.Render( writer );

        return true;
      }
      else
        return false;
    }

    private IHtmlElement MergeHead( IHtmlElement masterHead, IHtmlElement contentHead )
    {

      var head = masterHead.Document.CreateFragment().AddElement( "head" );

      head.AddCopy( contentHead.Elements().Where( e => e.Attribute( "ignore" ) == null ) );

      if ( !head.Find( "title" ).Any() )
      {
        var title = masterHead.Find( "title" ).FirstOrDefault();
        head.AddCopy( title );
      }


      {
        var existsStyleSheets =new HashSet<string>( head.Find( "link[rel=stylesheet]" ).Select( e => e.Attribute( "herf" ).Value() ), StringComparer.OrdinalIgnoreCase );
        foreach ( var element in masterHead.Find( "link[rel=stylesheet]" ) )
        {
          if ( !existsStyleSheets.Contains( element.Attribute( "href" ).Value() ) )
            head.AddCopy( element );
        }
      }

      return head;
    }


  }
}
