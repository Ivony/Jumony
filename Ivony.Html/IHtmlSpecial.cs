using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 特殊节点，其不是 HTML 的组成部分，例如代码块
  /// </summary>
  /// <remarks>
  /// 若此节点最终能生成 HTML 内容，应同时实现 IHtmlRenderable 接口。
  /// 特殊节点不被视为HTML的一部分，不认为会产生任何文本，也不自动生成HTML，而使用RawHtml原样输出
  /// </remarks>
  public interface IHtmlSpecial : IHtmlNode
  {
  }
}
