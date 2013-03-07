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


  /// <summary>
  /// HTML 注释的类别
  /// </summary>
  public enum HtmlCommentRenderType
  {
    /// <summary>普通注释，使用 &lt;!--内容--&gt; 表示，输出时自动格式化</summary>
    Normal,

    /// <summary>原样输出，输出时保持原样输出，但不会被任何解析器理解为文本</summary>
    AsIs,

    /// <summary>不会输出的注释，仅存在于 DOM 结构中，无法输出</summary>
    Unable
  }

}
