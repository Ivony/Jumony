using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
  public class HtmlCollection : IHtmlCollection
  {

    private IHtmlDocument _document;
    private object _sync = new object();
    private IList<IHtmlNode> _nodeCollection;

    public HtmlCollection( IHtmlDocument document )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );


      _document = document;
      _nodeCollection = new SynchronizedCollection<IHtmlNode>( SyncRoot );
    }

    public HtmlCollection( IEnumerable<IHtmlNode> nodes )
    {
      if ( nodes == null )
        throw new ArgumentNullException( "nodes" );

      if ( !nodes.Any() )
        throw new ArgumentException( "传入的节点集合不能是空的", "nodes" );


      _document = nodes.First().Document;

      nodes.ForAll( n => AddNode( n ) );

    }


    public void AddNode( IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( !object.Equals( node.Document, Document ) )
        throw new InvalidOperationException();

      node.EnsureAllocated();


      if ( node.IsDescendantOf( this ) )
        return;

      var container = node as IHtmlContainer;
      if ( container != null )
      {
        var descendants = _nodeCollection.Where( n => n.IsDescendantOf( container ) ).ToArray();
        descendants.ForAll( n => _nodeCollection.Remove( n ) );
      }

      var comparer = new NodeLocationComparer();
      var index = 0;
      var before = Nodes().LastOrDefault( n => comparer.Compare( n, node ) < 0 );
      if ( before != null )
        index = before.NodesIndexOfSelf() + 1;

      _nodeCollection.Insert( index, node );
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


  public class NodeLocationComparer : IComparer<IHtmlNode>
  {
    public int Compare( IHtmlNode x, IHtmlNode y )
    {
      if ( x == null )
        throw new ArgumentNullException( "x" );

      if ( y == null )
        throw new ArgumentNullException( "y" );

      if ( !object.Equals( x.Document, y.Document ) )
        throw new InvalidOperationException();

      x.EnsureAllocated();
      y.EnsureAllocated();

      if ( object.Equals( x, y ) )
        return 0;

      if ( object.Equals( x.Container, y.Container ) )
        return x.NodesIndexOfSelf() - y.NodesIndexOfSelf();


      var ancetors1 = x.Ancestors().Reverse().ToArray();
      var ancetors2 = y.Ancestors().Reverse().ToArray();

      int i = 0;
      while ( true )
      {

        if ( i > ancetors1.Length )
          return -1;

        if ( i > ancetors2.Length )
          return 1;

        if ( !object.Equals( ancetors1[i], ancetors2[i] ) )
          break;
      }

      return ancetors1[i].NodesIndexOfSelf() - ancetors2[i].NodesIndexOfSelf();
    }
  }

}
