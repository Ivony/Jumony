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




  /// <summary>
  /// 定义高级 HTML 节点容器，通过实现该接口可以自定义 Descendants 等方法的行为模式
  /// </summary>
  public interface IHtmlNodeCollection : IEnumerable<IHtmlNode>
  {
    /// <summary>
    /// 获取所有的元素
    /// </summary>
    /// <returns>该节点集合中所有的元素</returns>
    IEnumerable<IHtmlElement> Elements();

    /// <summary>
    /// 获取所有的子代元素
    /// </summary>
    /// <returns>该节点集合中所有的元素以及这些元素的子代元素</returns>
    IEnumerable<IHtmlElement> DescendantElements();

    /// <summary>
    /// 获取所有的子代节点
    /// </summary>
    /// <returns>该节点集合中所有的节点以及所有的子代节点</returns>
    IEnumerable<IHtmlNode> DescendantNodes();
  }


}
