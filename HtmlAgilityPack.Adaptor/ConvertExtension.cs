using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;


namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public static class ConvertExtension
  {

    public static IHtmlNode AsNode( this AP.HtmlNode node )
    {
      switch ( node.NodeType )
      {
        case AP.HtmlNodeType.Document:
          return new HtmlDocumentAdapter( node.OwnerDocument );
        case AP.HtmlNodeType.Element:
          return new HtmlElementAdapter( node );
        case AP.HtmlNodeType.Text:
          return new HtmlTextNodeAdapter( node );
        case AP.HtmlNodeType.Comment:
          return new HtmlCommentNodeAdapter( node );
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
        case AP.HtmlNodeType.Element:
          return (IHtmlContainer) node.AsNode();

        default:
          throw new ArgumentException( "只能从NodeType为Element或Document的HtmlNode转换", "node" );

      }
    }

    public static IHtmlElement AsElement( this AP.HtmlNode node )
    {
      switch ( node.NodeType )
      {
        case AP.HtmlNodeType.Element:
          return (IHtmlElement) node.AsNode();

        default:
          throw new ArgumentException( "只能从NodeType为Element的HtmlNode转换", "node" );
      }
    }



    public static IHtmlAttribute AsAttribute( this AP.HtmlAttribute attribute )
    {
      return new HtmlAttributeAdapter( attribute );
    }


  }
}
