using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Selectors
{
  /// <summary>
  /// 代表一个范围限定的选择器，
  /// </summary>
  public sealed class CssScopeSelector
  {

    public IHtmlContainer Scope
    {
      get;
      private set;
    }

  }
}
