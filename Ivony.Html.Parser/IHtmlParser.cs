using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 文档解析器
  /// </summary>
  public interface IHtmlParser
  {

    /// <summary>
    /// 分析 HTML 创建一个文档
    /// </summary>
    /// <param name="html">HTML 文本</param>
    /// <param name="uri">HTML 内容统一资源位置</param>
    /// <returns></returns>
    IHtmlDocument Parse( string html, Uri uri );

    /// <summary>
    /// 获取用于构建 DOM 结构的 DOM 提供程序
    /// </summary>
    IHtmlDomProvider DomProvider { get; }
  }
}
