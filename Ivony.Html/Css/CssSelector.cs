using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 实现 ICssSelector 接口
  /// </summary>
  public class CssSelector : ICssSelector
  {

    /// <summary>
    /// 创建 CssSelector 对象
    /// </summary>
    /// <param name="selector">选择器</param>
    /// <param name="specificity">选择器特异性</param>
    internal CssSelector( ISelector selector, CssSpecificity specificity )
    {
      Selector = selector;
      Specificity = specificity;
    }


    /// <summary>
    /// 选择器特异性
    /// </summary>
    public CssSpecificity Specificity
    {
      get;
      private set;
    }


    /// <summary>
    /// 选择器
    /// </summary>
    public ISelector Selector
    {
      get;
      private set;
    }


    /// <summary>
    /// 元素是否满足选择器要求
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool IsEligible( IHtmlElement element )
    {
      return Selector.IsEligible( element );
    }
  }
}
