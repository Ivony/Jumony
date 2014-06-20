using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  /// <summary>
  /// 定义 HTML 元素绑定器
  /// </summary>
  public interface IHtmlBinder
  {

    /// <summary>
    /// HTML 元素绑定器，对元素执行一些特定的数据绑定操作
    /// </summary>
    /// <param name="context">数据绑定上下文</param>
    /// <param name="element">要绑定的元素</param>
    void BindElement( HtmlBindingContext context, IHtmlElement element );

  }
}
