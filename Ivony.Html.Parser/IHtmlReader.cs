using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser.ContentModels;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// HTML 内容元素读取枚举器
  /// </summary>
  public interface IHtmlReader
  {

    /// <summary>
    /// 正在读取分析的 HTML 文本
    /// </summary>
    string HtmlText
    {
      get;
    }


    /// <summary>
    /// 获取 HTML 内容元素枚举器
    /// </summary>
    /// <returns>HTML 内容枚举器</returns>
    IEnumerable<HtmlContentFragment> EnumerateContent();

    /// <summary>
    /// 进入 CData 元素读取模式
    /// </summary>
    /// <remarks>读取器一旦进入CData元素读取模式，便会将除了元素结束标签之外的一切 HTML 内容当作文本返回。当遇到元素结束标签时，读取器应当自动退出 CData 元素读取模式</remarks>
    /// <param name="elementName">CData 元素名</param>
    void EnterCDataMode( string elementName );


  }
}
