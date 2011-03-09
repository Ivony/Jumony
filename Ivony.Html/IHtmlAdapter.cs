using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一种 HTML 转换器，其可以将 HTML 文档转换为文本。
  /// </summary>
  public interface IHtmlAdapter
  {

    /// <summary>
    /// 尝试呈现指定的节点
    /// </summary>
    /// <param name="node">要呈现的节点</param>
    /// <param name="writer">用于处理呈现结果的文本编写器</param>
    /// <returns>是否进行了自定义呈现，若返回false，则使用默认呈现</returns>
    bool Render( IHtmlNode node, TextWriter writer );

  }
}
