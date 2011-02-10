using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  public interface IHtmlParser
  {

    /// <summary>
    /// 分析 HTML 创建一个文档
    /// </summary>
    /// <param name="html">HTML文本</param>
    /// <returns></returns>
    IHtmlDocument Parse( string html );

  }
}
