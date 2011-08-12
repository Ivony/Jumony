using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个特殊的 HTML 标签内容
  /// </summary>
  public sealed class HtmlSpecialTag : HtmlContentFragment
  {

    /// <summary>
    /// 创建 HtmlSpecialTag 实例
    /// </summary>
    /// <param name="fragment"></param>
    /// <param name="content"></param>
    /// <param name="speciaSymbol"></param>
    public HtmlSpecialTag( HtmlContentFragment fragment, string content, string speciaSymbol )
      : base( fragment )
    {
      Content = content;
      SpecialSymbol = speciaSymbol;
    }

    public string Content
    {
      get;
      private set;
    }

    public string SpecialSymbol
    {
      get;
      private set;
    }

  }
}
