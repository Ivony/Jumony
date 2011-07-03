using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal class DomModifier : IHtmlDomModifier
  {

    private HtmlDocument _document;

    public DomModifier( HtmlDocument document )
    {
      _document = document;
    }



    private HtmlNode AddNode( IHtmlContainer container, int index, HtmlNode node )
    {
      var containerNode = container.RawObject as HtmlNode;

      if ( containerNode == null )
        throw new InvalidOperationException();

      containerNode.ChildNodes.Insert( index, node );

      return node;
    }


    #region IHtmlDomModifier 成员

    public IHtmlElement AddElement( IHtmlContainer container, int index, string name )
    {
      var node = new HtmlNode( HtmlNodeType.Element, _document, index );
      AddNode( container, index, node );

      return node.AsElement();
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      throw new NotImplementedException();
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      throw new NotImplementedException();
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      throw new NotImplementedException();
    }

    public void RemoveNode( IHtmlNode node )
    {
      throw new NotImplementedException();
    }

    public IHtmlAttribute AddAttribute( IHtmlElement element, string name, string value )
    {
      throw new NotImplementedException();
    }

    public void RemoveAttribute( IHtmlAttribute attribute )
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
