using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示一个抽象的 HTML 输入控件或控件组
  /// </summary>
  public interface IHtmlInputControl : IHtmlFormElement
  {

    string Name
    {
      get;
    }

  }


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
