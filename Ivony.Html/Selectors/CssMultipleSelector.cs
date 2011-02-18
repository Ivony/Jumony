using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 多重（并列）选择器
  /// </summary>
  internal sealed class CssMultipleSelector : ICssSelector
  {

    private ICssSelector[] _selectors;

    public CssMultipleSelector( params ICssSelector[] selectors )
    {

      _selectors = selectors;

    }


    /// <summary>
    /// 判断一个元素是否符合选择器要求
    /// </summary>
    /// <param name="element">要判断的元素</param>
    /// <returns>是否符合要求</returns>
    public bool IsEligible( IHtmlElement element )
    {
      return _selectors.Any( s => s.IsEligible( element ) );
    }


    /// <summary>
    /// 返回表示当前选择器的表达式
    /// </summary>
    /// <returns>表示当前选择器的表达式</returns>
    public override string ToString()
    {

      return string.Join( " , ", Array.ConvertAll( _selectors, s => s.ToString() ) );

    }


  }
}
