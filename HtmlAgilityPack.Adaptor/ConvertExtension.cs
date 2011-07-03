using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;
using Ivony.Fluent;


namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public static class ConvertExtension
  {

    public static IHtmlNode AsNode( this AP.HtmlNode node )
    {
      switch ( node.NodeType )
      {
        case AP.HtmlNodeType.Element:
          return new HtmlElementAdapter( node );
        case AP.HtmlNodeType.Text:
          return node.CastTo<AP.HtmlTextNode>().AsTextNode();
        case AP.HtmlNodeType.Comment:
          return node.CastTo<AP.HtmlCommentNode>().AsComment();
        default:
          throw new NotSupportedException();
      }
    }


    public static IHtmlDocument AsDocument( this AP.HtmlDocument document )
    {
      return new HtmlDocumentAdapter( document );
    }


    public static IHtmlContainer AsContainer( this AP.HtmlNode node )
    {

      switch ( node.NodeType )
      {
        case AP.HtmlNodeType.Document:
          return new HtmlDocumentAdapter( node.OwnerDocument );
        case AP.HtmlNodeType.Element:
          return new HtmlElementAdapter( node );

        default:
          throw new ArgumentException( "只能从NodeType为Element或Document的HtmlNode转换", "node" );

      }
    }

    public static IHtmlElement AsElement( this AP.HtmlNode node )
    {
      switch ( node.NodeType )
      {
        case AP.HtmlNodeType.Element:
          return new HtmlElementAdapter( node );

        default:
          throw new ArgumentException( "只能从NodeType为Element的HtmlNode转换", "node" );
      }
    }

    public static IHtmlTextNode AsTextNode( this AP.HtmlTextNode node )
    {
      return new HtmlTextNodeAdapter( node );
    }

    public static IHtmlComment AsComment( this AP.HtmlCommentNode node )
    {
      return new HtmlCommentNodeAdapter( node );
    }



    public static IHtmlAttribute AsAttribute( this AP.HtmlAttribute attribute )
    {
      return new HtmlAttributeAdapter( attribute );
    }


  }
}
