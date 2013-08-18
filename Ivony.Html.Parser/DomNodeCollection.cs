using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 实现一个 DomNode 的容器
  /// </summary>
  public sealed class DomNodeCollection : SynchronizedCollection<DomNode>
  {

    /// <summary>
    /// 所属的 HTML DOM 容器对象
    /// </summary>
    public IDomContainer Container
    {
      get;
      private set;
    }

    /// <summary>
    /// 创建 DomNodeCollection 对象
    /// </summary>
    /// <param name="container"></param>
    public DomNodeCollection( IDomContainer container )
      : base( container.SyncRoot )
    {
      Container = container;
    }

    /// <summary>
    /// 重写 InsertItem 方法，修改被插入节点的 Container 属性
    /// </summary>
    /// <param name="index">要插入的位置</param>
    /// <param name="item">要插入的节点</param>
    protected override void InsertItem( int index, DomNode item )
    {

      if ( item.Container != null )
        throw new InvalidOperationException();

      if ( index > Count )
        throw new ArgumentOutOfRangeException( "index" );


      if ( Count == 0 )
        AddDescendantNode( item );

      else if ( index == Count )
        AddDescendantNodeAfter( _descendants.LastItem, item );

      else
        AddDescendantNodeBefore( this[index], item );


      base.InsertItem( index, item );

      item.Container = Container;
    }

    private void AddDescendantNode( DomNode item )
    {
      _descendants.Add( item );

      var node = Container as DomNode;
      if ( node == null )
        return;

      else
        node.Container.CastTo<IDomContainer>().NodeCollection.AddDescendantNodeAfter( node, item );
    
    }

    private void AddDescendantNodeBefore( DomNode next, DomNode node )
    {

      _descendants.AddBefore( next, node );

      var parent = GetParent();
      if ( parent == null )
        return;

      else
        parent.NodeCollection.AddDescendantNodeBefore( next, node );

    }

    private void AddDescendantNodeAfter( DomNode previous, DomNode node )
    {
      _descendants.AddAfter( previous, node );

      var parent = GetParent();
      if ( parent == null )
        return;

      else
        parent.NodeCollection.AddDescendantNodeAfter( previous, node );
    }


    private IDomContainer GetParent()
    {
      var node = Container as IHtmlNode;
      if ( node == null )
        return null;

      else
        return node.Container as IDomContainer;
    }

    /// <summary>
    /// 重写 OnChanged 方法，在容器发生改变时清除元素列表缓存。
    /// </summary>
    protected override void OnChanged()
    {
      _elements = null;
    }

    internal void DescendantNodeRemoved( DomNode node )
    {
      _descendants.Remove( node );

      _descendantElements = null;
    }



    private SuperLinkedList<DomNode> _descendants = new SuperLinkedList<DomNode>();

    private IHtmlElement[] _elements;

    private IHtmlElement[] _descendantElements;


    private IEnumerable<IHtmlElement> Elements()
    {
      lock ( SyncRoot )
      {
        if ( _elements != null )
          return _elements.AsReadOnly();

        else
          return (_elements = this.OfType<DomElement>().ToArray()).AsReadOnly();
      }
    }

    private IEnumerable<IHtmlElement> DescendantElements()
    {
      lock ( SyncRoot )
      {
        if ( _descendantElements != null )
          return _descendantElements;

        else
          return _descendantElements = _descendants.OfType<DomElement>().ToArray();
      }
    }

    private IEnumerable<IHtmlNode> DescendantNodes()
    {
      return _descendants.Cast<IHtmlNode>();
    }


    private HtmlNodeCollection _nodeCollection;

    /// <summary>
    /// 获取实现了 IHtmlNodeCollection 类型的节点容器
    /// </summary>
    public IHtmlNodeCollection HtmlNodes
    {
      get 
      {
        lock ( SyncRoot )
        {
          if ( _nodeCollection == null )
            _nodeCollection = new HtmlNodeCollection( this );

          return _nodeCollection;
        }
      }
    }

    private class HtmlNodeCollection : IHtmlNodeCollection
    {

      private DomNodeCollection _collection;

      public HtmlNodeCollection( DomNodeCollection collection )
      {
        _collection = collection;
      }


      public IEnumerable<IHtmlElement> Elements()
      {
        return _collection.Elements();
      }

      public IEnumerable<IHtmlElement> DescendantElements()
      {
        return _collection.DescendantElements();
      }

      public IEnumerable<IHtmlNode> DescendantNodes()
      {
        return _collection.DescendantNodes();
      }

      public IEnumerator<IHtmlNode> GetEnumerator()
      {
        return _collection.Cast<IHtmlNode>().GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _collection.GetEnumerator();
      }
    }


  }
}
