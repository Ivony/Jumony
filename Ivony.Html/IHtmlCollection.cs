using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个 HTML 节点收集器，其可以收纳 HTML 节点，但不会修改 HTML 节点的 Container 属性。
  /// </summary>
  public interface IHtmlCollection : IHtmlContainer
  {
    void AddNode( IHtmlNode node );
  }


  public class HtmlCollection : IHtmlCollection, ICollection<IHtmlNode>
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

      var comparer = LocationExtensions.NodeLocationComparer;
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



    #region ICollection<IHtmlNode> 成员

    void ICollection<IHtmlNode>.Add( IHtmlNode item )
    {
      AddNode( item );
    }

    void ICollection<IHtmlNode>.Clear()
    {

    }

    public bool Contains( IHtmlNode item )
    {
      throw new NotImplementedException();
    }

    public void CopyTo( IHtmlNode[] array, int arrayIndex )
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove( IHtmlNode item )
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable<IHtmlNode> 成员

    public IEnumerator<IHtmlNode> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable 成员

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion
  }



}
