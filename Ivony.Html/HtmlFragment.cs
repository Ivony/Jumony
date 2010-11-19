using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;



namespace Ivony.Html
{

  /// <summary>
  /// HTML 文档碎片，游离节点的容器
  /// </summary>
  public class HtmlFragment : IHtmlContainer
  {


    private IList<IFreeNode> _nodes;

    private IHtmlNodeFactory _factory;

    private IHtmlDocument _document = null;


    public HtmlFragment( IHtmlNodeFactory factory )
    {
      _factory = factory;
      _nodes = new SynchronizedCollection<IFreeNode>( SyncRoot );
    }


    private readonly object sync = new object();

    public object SyncRoot
    {
      get { return sync; }
    }


    /// <summary>
    /// 向文档碎片中添加节点副本
    /// </summary>
    /// <param name="nodes">要添加的节点</param>
    /// <param name="factory">用于创建游离节点副本的节点工厂</param>
    public void AddNodesCopy( IEnumerable<IHtmlNode> nodes, IHtmlNodeFactory factory )
    {
      AddNodes( nodes.Select( n => factory.MakeCopy( n ) ) );
    }


    /// <summary>
    /// 向文档碎片中添加游离节点
    /// </summary>
    /// <param name="nodes">要添加的游离节点</param>
    public void AddNodes( IEnumerable<IFreeNode> nodes )
    {
      lock ( SyncRoot )
      {
        nodes.ForAll( n => AddNode( n ) );
      }
    }

    public void AddNode( IFreeNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );


      lock ( SyncRoot )
      {

        if ( _document == null )
          _document = node.Document;


        if ( !_document.Equals( node.Document ) )
          throw new InvalidOperationException();

        _nodes.Add( node );

      }
    }

    public void InsertTo( IHtmlContainer container, int index )
    {
      lock ( SyncRoot )
      {
        foreach ( var node in _nodes.Reverse().ToArray() )
        {
          node.Into( container, index );
          _nodes.Remove( node );
        }
      }
    }

    public IHtmlNodeFactory Factory
    {
      get { return _factory; }
    }



    #region IHtmlNode 成员

    IHtmlContainer IHtmlNode.Container
    {
      get { return null; }
    }

    object IHtmlObject.NodeObject
    {
      get { return this; }
    }

    IHtmlDocument IHtmlObject.Document
    {
      get { return _document; }
    }


    void IHtmlNode.Remove()
    {
      throw new InvalidOperationException();
    }

    string IHtmlNode.RawHtml
    {
      get
      {
        var builder = new StringBuilder();

        foreach ( IHtmlNode node in _nodes )
        {
          if ( node.RawHtml == null )
            return null;

          builder.Append( node.RawHtml );
        }

        return builder.ToString();
      }
    }

    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlNodeContainer.Nodes()
    {
      return _nodes.Cast<IHtmlNode>();
    }

    #endregion



  }
}
