using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomModifier : IHtmlDomModifier
  {

    public IHtmlElement AddElement( IHtmlContainer container, int index, string name )
    {
      var element = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomElement( name, null ) );

      return element;
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      var textNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomTextNode( htmlText ) );

      return textNode;
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      var commentNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomComment( comment ) );

      return commentNode;
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      //UNDONE
      throw new NotSupportedException();
    }

    public void RemoveNode( IHtmlNode node )
    {
      EnsureDomNode( node ).Remove();
    }

    private DomNode EnsureDomNode( IHtmlNode node )
    {
      var domNode = node as DomNode;

      if ( domNode == null )
        throw new NotSupportedException( "只能移除特定类型节点" );

      return domNode;
    }



    public IHtmlAttribute AddAttribute( IHtmlElement element, string name )
    {
      var domElement = element as DomElement;
      if ( domElement == null )
        throw new InvalidOperationException();

      return domElement.AddAttribute( name );
    }

    public void RemoveAttribute( IHtmlAttribute attribute )
    {
      var domAttribute = attribute as DomAttribute;
      if ( domAttribute == null )
        throw new InvalidOperationException();

      domAttribute.Remove();
    }

  }
}
