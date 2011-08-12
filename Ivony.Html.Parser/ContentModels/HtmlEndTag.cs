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
