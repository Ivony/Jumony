using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// 代表一个范围限定的选择器，
  /// </summary>
  public sealed class CssScopedSelector : ICssSelector
  {
    public CssScopedSelector( ICssSelectorWithScope selector, IHtmlContainer scope )
    {
      Selector = selector;
      Scope = scope;
    }

    public IHtmlContainer Scope
    {
      get;
      private set;
    }


    public ICssSelectorWithScope Selector
    {
      get;
      private set;
    }


    public bool IsEligible( IHtmlElement element )
    {
      return Selector.IsEligible( element, Scope );
    }
  }
}
