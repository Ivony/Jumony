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
    /// <param name="html">HTML 文本</param>
    /// <param name="url">HTML 内容统一资源位置</param>
    /// <returns></returns>
    IHtmlDocument Parse( string html, Uri url );


  }
}
