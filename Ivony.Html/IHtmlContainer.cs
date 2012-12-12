using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个 HTML 内容容器，作为元素和文档的抽象
  /// </summary>
  public interface IHtmlContainer : IHtmlDomObject
  {

    /// <summary>
    /// 获取所有子节点
    /// </summary>
    /// <returns>容器的子节点</returns>
    IEnumerable<IHtmlNode> Nodes();


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    object SyncRoot
    {
      get;
    }


  }

}
