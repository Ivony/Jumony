using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 否定伪类实现
  /// </summary>
  internal class NegationPseudoClass : ICssPseudoClassSelector
  {
    private CssElementSelector _elementSelector;


    /// <summary>
    /// 构建 NegationPseudoClass 对象
    /// </summary>
    /// <param name="elementSelector">元素选择器</param>
    public NegationPseudoClass( CssElementSelector elementSelector )
    {
      _elementSelector = elementSelector;
    }


    /// <summary>
    /// 检测是否满足伪类选择器
    /// </summary>
    /// <param name="element">要检测的元素</param>
    /// <returns>是否满足选择器</returns>
    public bool IsEligible( IHtmlElement element )
    {
      return !_elementSelector.IsEligible( element );
    }


    /// <summary>
    /// 获取否定伪类的字符串表达形式
    /// </summary>
    /// <returns>否定伪类的字符串表达形式</returns>
    public override string ToString()
    {
      return string.Format( ":not({0})", _elementSelector );
    }
  }
}
