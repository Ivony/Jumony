using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomModifier : IHtmlDomModifier
  {

    private DomDocument _document;

    public DomModifier( DomDocument document )
    {
      _document = document;
    }


    public IHtmlElement AddElement( IHtmlContainer container, int index, string name )
    {
      var element = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomElement( name, null ) );
      OnDomChanged( element );
      return element;
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      var textNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomTextNode( htmlText ) );
      OnDomChanged( textNode );
      return textNode;
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      var commentNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomComment( comment ) );
      OnDomChanged( commentNode );
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

      OnDomChanged( domElement );

      return domElement.AddAttribute( name );
    }

    public void RemoveAttribute( IHtmlAttribute attribute )
    {
      var domAttribute = attribute as DomAttribute;
      if ( domAttribute == null )
        throw new InvalidOperationException();

      OnDomChanged( domAttribute.Element );

      domAttribute.Remove();
    }

    public IHtmlDocument Document
    {
      get { throw new NotImplementedException(); }
    }



    public bool SupportsNotifyChange
    {
      get { return true; }
    }


    protected virtual void OnDomChanged( DomNode node )
    {
      if ( DomChanged != null )
        DomChanged( this, new HtmlNodeEventArgs( node ) );
    }

    public event EventHandler<HtmlNodeEventArgs>  DomChanged;

  }
}
