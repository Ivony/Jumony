using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 CSS 选择器
  /// </summary>
  public interface ICssSelector : ISelector
  {

    /// <summary>
    /// CSS 选择器特异性
    /// </summary>
    CssSpecificity Specificity { get; }

  }
}
