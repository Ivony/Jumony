using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Selectors
{
  public interface ICssScopeSelector
  {
    bool IsEligible( IHtmlElement element, IHtmlContainer container );
  }

  public interface ICssSelector
  {
    bool IsEligible( IHtmlElement element );
  }
}
