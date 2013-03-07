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
  public class DomNodeCollection : SynchronizedCollection<DomNode>
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
    /// 重写 InsertItem 方法，修改被插入节点的 Container 属相
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected override void InsertItem( int index, DomNode item )
    {

      if ( item.Container != null )
        throw new InvalidOperationException();

      base.InsertItem( index, item );

      item.Container = Container;

    }


    /// <summary>
    /// 获取所有节点
    /// </summary>
    public IEnumerable<IHtmlNode> HtmlNodes
    {
      get { return this.Cast<IHtmlNode>().AsReadOnly(); }
    }

  }
}
