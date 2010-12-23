using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public interface ICssSelector
  {

    bool IsEligible( IHtmlElement element );

  }

  public interface ICssScopedSelector
  {

    bool IsEligible( IHtmlElement element, IHtmlContainer scope );

  }
}
