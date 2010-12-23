using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 多重（并列）选择器
  /// </summary>
  internal sealed class CssMultipleSelector : ICssSelectorWithScope
  {

    private CssCasecadingSelector[] _selectors;

    public CssMultipleSelector( params CssCasecadingSelector[] selectors )
    {

      _selectors = selectors;

    }


    public bool IsEligible( IHtmlElement element, IHtmlContainer scope )
    {
      return _selectors.Any( s => s.IsEligible( element, scope ) );
    }

  }
}
