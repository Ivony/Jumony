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
    /// <param name="fragment"></param>
    /// <param name="comment"></param>
    public HtmlCommentContent( HtmlContentFragment fragment, string comment )
      : base( fragment )
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
