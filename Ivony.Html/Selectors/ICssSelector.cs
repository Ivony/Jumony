using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 代表一个CSS选择器的抽象
  /// </summary>
  public interface ICssSelector
  {

    bool IsEligible( IHtmlElement element );

  }

  /// <summary>
  /// 带有范围限制的CSS选择器
  /// </summary>
  public interface ICssSelectorWithScope
  {

    bool IsEligible( IHtmlElement element, IHtmlContainer scope );

  }
}
