using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{


  /// <summary>
  /// DOM 结构修改器
  /// </summary>
  public class DomModifier : IHtmlDomModifier, INotifyDomChanged
  {

    /// <summary>
    /// 修改文档的 URI
    /// </summary>
    /// <param name="document">要修改的文档</param>
    /// <param name="uri">新的文档 URI</param>
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

      OnDomChanged( this, new HtmlDomChangedEventArgs( element, container, HtmlDomChangedAction.Add ) );

      return element;
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      var textNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomTextNode( htmlText ) );

      OnDomChanged( this, new HtmlDomChangedEventArgs( textNode, container, HtmlDomChangedAction.Add ) );

      return textNode;
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      var commentNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomComment( comment ) );

      OnDomChanged( this, new HtmlDomChangedEventArgs( commentNode, container, HtmlDomChangedAction.Add ) );

      return commentNode;
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      var specialNode = DomProvider.EnsureDomContainer( container ).InsertNode( index, new DomSpecial( html ) );

      //UNDONE 未确定special node具体是什么

      OnDomChanged( this, new HtmlDomChangedEventArgs( specialNode, container, HtmlDomChangedAction.Add ) );

      return specialNode;
    }

    public void RemoveNode( IHtmlNode node )
    {
      var container = node.Container;

      EnsureDomNode( node ).Remove();

      OnDomChanged( this, new HtmlDomChangedEventArgs( node, container, HtmlDomChangedAction.Remove ) );
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



    internal void OnFragmentInto( DomFragment fragment, IHtmlContainer targetContainer, DomNode node )
    {
      OnDomChanged( this, new HtmlDomChangedEventArgs( node, fragment, HtmlDomChangedAction.Remove ) );
      OnDomChanged( this, new HtmlDomChangedEventArgs( node, targetContainer, HtmlDomChangedAction.Add ) );
    }

    protected virtual void OnDomChanged( object sender, HtmlDomChangedEventArgs e )
    {
      if ( HtmlDomChanged != null )
        HtmlDomChanged( sender, e );
    }

    public event EventHandler<HtmlDomChangedEventArgs>  HtmlDomChanged;
  }
}
