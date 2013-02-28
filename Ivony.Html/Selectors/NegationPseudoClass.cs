using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Selectors
{
  internal class NegationPseudoClass : ICssPseudoClassSelector
  {
    private CssElementSelector _elementSelector;

    public NegationPseudoClass( CssElementSelector elementSelector )
    {
      _elementSelector = elementSelector;
    }

    public bool IsEligible( IHtmlElement element )
    {
      return !_elementSelector.IsEligible( element );
    }
  }
}
