using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 专门用于 Web UI 引擎的 HTML 解析器
  /// </summary>
  public class WebParser : JumonyParser
  {

    /// <summary>
    /// 重写此方法以确保 &lt;partial&gt; 标签被视为CData元素。
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    protected override bool IsCDataElement( Parser.ContentModels.HtmlBeginTag tag )
    {

      if ( tag.TagName.EqualsIgnoreCase( "partial" ) )
        return true;
      
      return base.IsCDataElement( tag );
    }

  }
}
