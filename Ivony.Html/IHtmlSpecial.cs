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
  /// 若此节点最终能生成 HTML 内容，应同时实现 IHtmlRenderable 接口
  /// </remarks>
  public interface IHtmlSpecial : IHtmlNode
  {
  }
}
