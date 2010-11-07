using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示一个抽象的表单控件
  /// </summary>
  public interface IHtmlFormElement
  {
    HtmlForm Form
    {
      get;
    }
  }

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
  /// 表示一个可以获得焦点的表单控件
  /// </summary>
  public interface IHtmlFocusableControl : IHtmlFormElement
  {
    string ElementId
    {
      get;
    }
  }
}
