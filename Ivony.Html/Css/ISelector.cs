using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 元素选择器的抽象
  /// </summary>
  public interface ISelector
  {
    /// <summary>
    /// 判断一个元素是否符合选择器要求
    /// </summary>
    /// <param name="element">要判断的元素</param>
    /// <returns>是否符合要求</returns>
    bool IsEligible( IHtmlElement element );
  }
}
