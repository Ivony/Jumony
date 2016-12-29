using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{

  /// <summary>
  /// 实现 HTML 节点容器
  /// </summary>
  public sealed class HtmlNodeContainer : IReadOnlyList<HtmlNode>
  {

    List<HtmlNode> _nodes;


    internal HtmlNodeContainer( IHtmlContainer container )
    {
      Container = container;
    }


    /// <summary>
    /// 获取容器对象
    /// </summary>
    public IHtmlContainer Container { get; }


    /// <summary>
    /// 获取指定索引位置的节点
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns></returns>
    public HtmlNode this[int index]
    {
      get
      {
        throw new NotImplementedException();
      }
    }


    /// <summary>
    /// 获取节点数量
    /// </summary>
    public int Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }


    IEnumerator<HtmlNode> IEnumerable<HtmlNode>.GetEnumerator()
    {
      return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _nodes.GetEnumerator();
    }
  }
}
