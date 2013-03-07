using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个 HTML 结束标签
  /// </summary>
  public sealed class HtmlEndTag : HtmlContentFragment
  {

    /// <summary>
    /// 创建 HtmlTextContent 对象
    /// </summary>
    /// <param name="info">应当被认为是 HTML 结束标签的 HTML 片段</param>
    /// <param name="tagName">标签名</param>
    public HtmlEndTag( HtmlContentFragment info, string tagName )
      : base( info )
    {
      TagName = tagName;
    }


    /// <summary>
    /// 标签名
    /// </summary>
    public string TagName
    {
      get;
      private set;
    }


  }
}
