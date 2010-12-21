using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Selectors
{

  /// <summary>
  /// 多重（并列）选择器
  /// </summary>
  internal class CssMultipleSelector
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
