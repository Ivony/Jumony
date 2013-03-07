using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 所有CSS伪类选择器需要实现的接口
  /// </summary>
  /// <remarks>
  /// 对实现者的说明：CSS所有选择器实例都应当是线程安全的，请在实现时满足这一限制
  /// </remarks>
  public interface ICssPseudoClassSelector
  {
    /// <summary>
    /// 判断一个元素是否符合选择器要求
    /// </summary>
    /// <param name="element">要判断的元素</param>
    /// <returns>是否符合要求</returns>
    bool IsEligible( IHtmlElement element );
  }
}
