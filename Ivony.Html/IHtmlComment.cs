using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个 HTML 注释，或应当被忽略的 HTML 内容
  /// </summary>
  public interface IHtmlComment : IHtmlNode
  {
    /// <summary>
    /// 注释文本
    /// </summary>
    string Comment
    {
      get;
    }
  }
}
