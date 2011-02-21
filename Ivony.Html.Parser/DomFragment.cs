using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;
using Ivony.Fluent;

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

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( !object.Equals( container.Document, Document ) )
        throw new InvalidOperationException();

      var domContainer = container as IDomContainer;
      if ( domContainer == null )
        throw new InvalidOperationException();

      lock ( SyncRoot )
      {

        var nodeList = NodeCollection.ToArray();

        lock ( container.SyncRoot )
        {
          foreach ( var node in nodeList.Reverse() )
          {
            node.Container = null;
            NodeCollection.Remove( node );

            domContainer.NodeCollection.Insert( index, node );
          }
        }
      }
    }
  }


  public class DomCollection : IHtmlCollection
  {

    private DomDocument _document;
    private object _sync = new object();
    private IList<DomNode> _nodeCollection;

    public DomCollection( DomDocument document )
    {
      _document = document;
      _nodeCollection = new SynchronizedCollection<DomNode>( SyncRoot );
    }


    public void AddNode( IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( !object.Equals( node.Document, Document ) )
        throw new InvalidOperationException();

      if ( node.IsDescendantOf( this ) )
        return;

      var container = node as IHtmlContainer;
      if ( container != null )
      {
        var descendants = _nodeCollection.Where( n => n.IsDescendantOf( container ) ).ToArray();
        descendants.ForAll( n => _nodeCollection.Remove( n ) );
      }


    }

    public IEnumerable<IHtmlNode> Nodes()
    {
      return _nodeCollection.Cast<IHtmlNode>().AsReadOnly();
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


  }

  public class DomNodeLocationComparer : IComparer<DomNode>
  {
    public int Compare( DomNode x, DomNode y )
    {
      if ( x == null )
        throw new ArgumentNullException( "x" );

      if ( y == null )
        throw new ArgumentNullException( "y" );

      if ( !object.Equals( x.Document, y.Document ) )
        throw new InvalidOperationException();

      if ( object.Equals( x, y ) )
        return 0;

      if ( object.Equals( x.Container, y.Container ) )
        return x.NodesIndexOfSelf() - y.NodesIndexOfSelf();


      var ancetors1 = x.Ancestors().Reverse().ToArray();
      var ancetors2 = y.Ancestors().Reverse().ToArray();

      int i = 0;
      while ( true )
      {

        if ( i >= ancetors1.Length )
          return -1;

        if ( i >= ancetors2.Length )
          return 1;




        if ( !object.Equals( ancetors1[i], ancetors2[i] ) )
          break;
      }





    }
  }


}


