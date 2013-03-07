using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;



namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 文档碎片，未分配节点的容器，文档碎片可以再次被分配到 DOM 上。
  /// </summary>
  public interface IHtmlFragment : IHtmlContainer
  {
    /// <summary>
    /// 将碎片插入到文档指定位置
    /// </summary>
    /// <param name="container">要插入的容器</param>
    /// <param name="index">要插入的位置</param>
    /// <returns>固定于文档上的节点集</returns>
    IEnumerable<IHtmlNode> Into( IHtmlContainer container, int index );
  }

}
