using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 定义特定于 Jumony Parser 的 IHtmlContainer 实现规范。
  /// </summary>
  public interface IDomContainer : IHtmlContainer
  {
    /// <summary>
    /// HTML 节点容器
    /// </summary>
    DomNodeCollection NodeCollection
    {
      get;
    }

  }

  internal static class DomContianerExtension
  {
    public static T AddNode<T>( this IDomContainer container, T node ) where T : DomNode
    {
      container.NodeCollection.Add( node );
      return node;
    }

    public static T InsertNode<T>( this IDomContainer container, int index, T node ) where T : DomNode
    {
      container.NodeCollection.Insert( index, node );
      return node;
    }

  }
}
