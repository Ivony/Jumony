using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  public class DomModifier : IHtmlDomModifier, INotifyDomChanged
  {

    public void ResolveUri( IHtmlDocument document, Uri uri )
    {

      var domDocument = document as DomDocument;

      if ( domDocument == null )
        throw new InvalidOperationException();

      domDocument.DocumentUri = uri;
    }


    public IHtmlElement AddElement( IHtmlContainer container, int index, string name )
    {
      var element = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomElement( name, null ) );

      OnDomChanged( this, new HtmlNodeEventArgs( element, HtmlDomChangedAction.Add ) );

      return element;
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      var textNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomTextNode( htmlText ) );

      OnDomChanged( this, new HtmlNodeEventArgs( textNode, HtmlDomChangedAction.Add ) );

      return textNode;
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      var commentNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomComment( comment ) );

      OnDomChanged( this, new HtmlNodeEventArgs( commentNode, HtmlDomChangedAction.Add ) );

      return commentNode;
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      var specialNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomSpecial( html ) );

      //UNDONE 未确定special node具体是什么

      OnDomChanged( this, new HtmlNodeEventArgs( specialNode, HtmlDomChangedAction.Add ) );

      return specialNode;
    }

    public void RemoveNode( IHtmlNode node )
    {
      EnsureDomNode( node ).Remove();

      OnDomChanged( this, new HtmlNodeEventArgs( node, HtmlDomChangedAction.Remove ) );
    }

    private DomNode EnsureDomNode( IHtmlNode node )
    {
      var domNode = node as DomNode;

      if ( domNode == null )
        throw new NotSupportedException( "只能操作特定类型节点" );

      return domNode;
    }



    public IHtmlAttribute AddAttribute( IHtmlElement element, string name, string value )
    {
      var domElement = element as DomElement;
      if ( domElement == null )
        throw new InvalidOperationException();

      return domElement.AddAttribute( name, value );
    }

    public void RemoveAttribute( IHtmlAttribute attribute )
    {
      var domAttribute = attribute as DomAttribute;
      if ( domAttribute == null )
        throw new InvalidOperationException();

      domAttribute.Remove();
    }


    protected virtual void OnDomChanged( object sender, HtmlNodeEventArgs e )
    {
      if ( HtmlDomChanged != null )
        HtmlDomChanged( sender, e );
    }

    public event EventHandler<HtmlNodeEventArgs>  HtmlDomChanged;
  }
}
