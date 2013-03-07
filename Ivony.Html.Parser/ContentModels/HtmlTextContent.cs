using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  /// <summary>
  /// 描述一段 HTML 文本
  /// </summary>
  public sealed class HtmlTextContent : HtmlContentFragment
  {
    /// <summary>
    /// 创建 HtmlTextContent 对象
    /// </summary>
    /// <param name="fragement">应当被认为是 HTML 文本的 HTML 片段</param>
    public HtmlTextContent( HtmlContentFragment fragement ) : base( fragement ) { }
  }
}
