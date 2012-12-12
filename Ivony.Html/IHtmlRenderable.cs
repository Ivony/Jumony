using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html
{

  /// <summary>
  /// 要自定义呈现逻辑的 HTML 节点所需要实现的接口
  /// </summary>
  public interface IHtmlRenderableNode : IHtmlNode
  {

    /// <summary>
    /// 呈现这个节点
    /// </summary>
    /// <param name="writer">用于处理呈现结果的文本编写器</param>
    void Render( TextWriter writer );

  }
}
