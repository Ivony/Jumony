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
    IEnumerable<IHtmlNode> Nodes();
  }
}
