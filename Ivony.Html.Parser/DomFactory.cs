using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;

namespace Ivony.Html.Parser
{


  public class DomFragment : IHtmlFragment, IDomContainer
  {

    public DomFragment( DomDocument document )
    {
      _document = document;
      _nodeCollection = new DomNodeCollection( this );
    }

    private object _sync = new object();
    private DomDocument _document;
    private DomNodeCollection _nodeCollection;

    public DomNodeCollection NodeCollection
    {
      get { return _nodeCollection; }
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      return _nodeCollection.HtmlNodes;
    }

    public object RawObject
    {
      get { return this; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public IHtmlDocument Document
    {
      get { return _document; }
    }

    public object SyncRoot
    {
      get { return _sync; }
    }







    public IHtmlElement AddElement( int index, string name, IDictionary<string, string> attributes )
    {
      lock ( SyncRoot )
      {
        var element = new DomElement( name, attributes );
        NodeCollection.Insert( index, element );
        return element;
      }
    }

    public IHtmlTextNode AddTextNode( int index, string htmlText )
    {
      lock ( SyncRoot )
      {
        var textNode = new DomTextNode( htmlText );
        NodeCollection.Insert( index, textNode );
        return textNode;
      }
    }

    public IHtmlComment AddComment( int index, string comment )
    {
      lock ( SyncRoot )
      {
        var commentNode = new DomComment( comment );
        NodeCollection.Insert( index, commentNode );
        return commentNode;
      }
    }

    public IHtmlSpecial AddSpecial( int index, string html )
    {
      throw new NotSupportedException();
    }

    public void Into( IHtmlContainer container, int index )
    {
      lock ( SyncRoot )
      {

        var nodeList = NodeCollection.ToArray();

        Array.Reverse( nodeList );

        lock ( container.SyncRoot )
        {
          foreach ( var node in nodeList )
          {
            node.Container = null;
          }
        }
      }
    }
  }
}


