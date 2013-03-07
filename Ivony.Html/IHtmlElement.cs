using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 元素
  /// </summary>
  public interface IHtmlElement : IHtmlNode, IHtmlContainer
  {

    /// <summary>
    /// 元素名
    /// </summary>
    string Name
    {
      get;
    }


    /// <summary>
    /// 获取元素的所有属性
    /// </summary>
    /// <returns>元素的所有属性</returns>
    IEnumerable<IHtmlAttribute> Attributes();

  }
}
