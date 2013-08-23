using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 限制为仅匹配特定一些些元素的 CSS 选择器，主要用于作为范畴限定。
  /// </summary>
  public class CssElementsRestrictionSelector : ISelector
  {

    private readonly HashSet<IHtmlElement> _elements;

    /// <summary>
    /// 创建 CssElementsRestrictionSelector 对象
    /// </summary>
    /// <param name="elements">特定的元素</param>
    public CssElementsRestrictionSelector( IEnumerable<IHtmlElement> elements )
    {

      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      _elements = new HashSet<IHtmlElement>( elements );

    }

    bool ISelector.IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;

      return _elements.Contains( element );
    }

    /// <summary>
    /// 获取选择器的字符串表达形式
    /// </summary>
    /// <returns>总是返回 "#elements#" 字符串</returns>
    public override string ToString()
    {
      return "#elements#";
    }
  }
}
