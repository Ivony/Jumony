using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个 HTML 注释内容片段
  /// </summary>
  public sealed class HtmlCommentContent : HtmlContentFragment
  {

    /// <summary>
    /// 创建 HtmlCommentContent 实例
    /// </summary>
    /// <param name="info">应当被认为是 HTML 结束标签的 HTML 片段</param>
    /// <param name="comment">注释内容</param>
    public HtmlCommentContent( HtmlContentFragment info, string comment )
      : base( info )
    {
      Comment = comment;
    }

    /// <summary>
    /// 注释内容
    /// </summary>
    public string Comment
    {
      get;
      private set;
    }

  }
}
