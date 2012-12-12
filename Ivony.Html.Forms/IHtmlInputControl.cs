using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示一个文本输入控件
  /// </summary>
  public interface IHtmlTextControl : IHtmlInputControl, IHtmlFocusableControl
  {

    string TextValue
    {
      get;
      set;
    }
  }
}
