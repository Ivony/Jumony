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


    private IHtmlNodeFactory _factory;


    public HtmlFragment( IHtmlNodeFactory factory )
    {
      _factory = factory;

      _nodes = new SynchronizedCollection<IFreeNode>( SyncRoot );
    }


    private readonly object sync = new object();

    /// <summary>
    /// 获取用于同步操作的对象
    /// </summary>
    public object SyncRoot
    {
      get { return sync; }
    }



    private SynchronizedCollection<IFreeNode> _nodes;

    /// <summary>
    /// 获取所有游离节点
    /// </summary>
    protected SynchronizedCollection<IFreeNode> Nodes
    {
      get { return _nodes; }
    }





    /// <summary>
    /// 向文档碎片中添加游离节点
    /// </summary>
    /// <param name="node">要添加的游离节点</param>
    /// <returns>文档碎片自身</returns>
    public virtual HtmlFragment AddNode( IFreeNode node )
    {
      CanAdded( node );

      Nodes.Add( node );

      return this;
    }


    /// <summary>
    /// 向文档碎片中添加游离节点
    /// </summary>
    /// <param name="index">要添加的位置</param>
    /// <param name="node">要添加的游离节点</param>
    /// <returns>文档碎片自身</returns>
    public virtual HtmlFragment AddNode( int index, IFreeNode node )
    {
      CanAdded( node );

      Nodes.Insert( index, node );

      return this;
    }


    /// <summary>
    /// 向文档碎片中添加游离节点
    /// </summary>
    /// <param name="nodes">要添加的游离节点</param>
    /// <returns>文档碎片自身</returns>
    public HtmlFragment AddNodes( IEnumerable<IFreeNode> nodes )
    {

      lock ( SyncRoot )
      {
        nodes.ForAll( n => AddNode( n ) );
      }

      return this;
    }




    /// <summary>
    /// 向文档碎片中添加节点本地副本
    /// </summary>
    /// <param name="node">要添加副本的节点</param>
    /// <returns>文档碎片自身</returns>
    public HtmlFragment AddCopy( IHtmlNode node )
    {
      AddNode( MakeCopy( node ) );

      return this;
    }

    /// <summary>
    /// 向文档碎片中添加节点本地副本
    /// </summary>
    /// <param name="index">要添加的位置</param>
    /// <param name="node">要添加副本的节点</param>
    /// <returns>文档碎片自身</returns>
    public HtmlFragment AddCopy( int index, IHtmlNode node )
    {
      AddNode( index, MakeCopy( node ) );

      return this;
    }


    /// <summary>
    /// 向文档碎片中添加节点本地副本
    /// </summary>
    /// <param name="nodes">要添加的节点</param>
    /// <returns>文档碎片自身</returns>
    public HtmlFragment AddCopies( IEnumerable<IHtmlNode> nodes )
    {
      nodes.ForAll( n => AddCopy( n ) );

      return this;
    }




    /// <summary>
    /// 创建指定节点的本地副本
    /// </summary>
    /// <param name="node">要创建副本的节点</param>
    /// <returns>创建的本地副本</returns>
    protected IFreeNode MakeCopy( IHtmlNode node )
    {
      return Factory.MakeCopy( node );
    }



    /// <summary>
    /// 检查节点是否可以被加入文档碎片。
    /// </summary>
    /// <param name="node">要检查的节点</param>
    public virtual void CanAdded( IFreeNode node )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( !node.Factory.Document.Equals( Factory.Document ) )
        throw new InvalidOperationException( "不能添加另一文档的游离节点" );
    }




    /// <summary>
    /// 将文档碎片插入到容器的指定位置
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="index">位置</param>
    public IEnumerable<IHtmlNode> InsertTo( IHtmlContainer container, int index )
    {

      var result = new List<IHtmlNode>();

      lock ( SyncRoot )
      {
        foreach ( var node in _nodes.Reverse().ToArray() )
        {
          result.Add( node.Into( container, index ) );
          _nodes.Remove( node );
        }
      }

      return result.AsReadOnly();
    }


    /// <summary>
    /// 用于创建游离节点的创建器
    /// </summary>
    public IHtmlNodeFactory Factory
    {
      get { return _factory; }
    }



    #region IHtmlNode 成员

    object IHtmlDomObject.RawObject
    {
      get { return this; }
    }

    IHtmlDocument IHtmlDomObject.Document
    {
      get { return Factory.Document; }
    }


    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Nodes.Cast<IHtmlNode>().AsReadOnly();
    }

    #endregion

    /// <summary>
    /// 获取节点的容器，此属性始终返回 null
    /// </summary>
    public IHtmlContainer Container
    {
      get { return null; }
    }

    /// <summary>
    /// 获取原始 HTML，始终返回 null
    /// </summary>
    public string RawHtml
    {
      get { return null; }
    }

    /// <summary>
    /// 从 DOM 中移除节点，此方法始终抛出 NotSupportedException 异常
    /// </summary>
    public void Remove()
    {
      throw new NotSupportedException();
    }
  }
}
