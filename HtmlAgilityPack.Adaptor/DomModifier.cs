using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAP = HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal class DomModifier : IHtmlDomModifier
  {

    private HAP.HtmlDocument _document;

    public DomModifier( HAP.HtmlDocument document )
    {
      _document = document;
    }



    private HAP.HtmlNode AddNode( IHtmlContainer container, int index, HAP.HtmlNode node )
    {
      var containerNode = container.RawObject as HAP.HtmlNode;

      if ( containerNode == null )
        throw new InvalidOperationException();

      containerNode.ChildNodes.Insert( index, node );

      return node;
    }


    #region IHtmlDomModifier 成员

    public IHtmlElement AddElement( IHtmlContainer container, int index, string name )
    {
      var element = _document.CreateElement( name );
      AddNode( container, index, element );

      return element.AsElement();
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      var node = _document.CreateTextNode( htmlText );

      AddNode( container, index, node );

      return node.AsTextNode();
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      var node = _document.CreateComment( comment );

      AddNode( container, index, node );

      return node.AsComment();

    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      throw new NotSupportedException();
    }

    public void RemoveNode( IHtmlNode node )
    {
      var _node = node.RawObject as HAP.HtmlNode;

      if ( _node == null )
        throw new ArgumentException( "node" );

      _node.Remove();
    }

    public IHtmlAttribute AddAttribute( IHtmlElement element, string name, string value )
    {
      var attribute = _document.CreateAttribute( name, value );

      var elementNode = element.RawObject as HAP.HtmlNode;
      if ( elementNode == null )
        throw new ArgumentException( "element" );

      elementNode.Attributes.Add( attribute );

      return attribute.AsAttribute();
    }

    public void RemoveAttribute( IHtmlAttribute attribute )
    {

      var htmlAttribute = attribute.RawObject as HAP.HtmlAttribute;

      if ( htmlAttribute == null )
        throw new ArgumentException( "attribute" );

      htmlAttribute.Remove();
    }

    #endregion
  }
}
