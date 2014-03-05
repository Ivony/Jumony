using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  /// <summary>
  /// 定义 HTML 元素绑定器
  /// </summary>
  public interface IHtmlElementBinder
  {

    /// <summary>
    /// 绑定元素
    /// </summary>
    /// <param name="context">数据绑定上下文</param>
    /// <param name="element">要绑定的元素</param>
    /// <returns>返回一个值，是否禁止后面的绑定器的执行</returns>
    void BindElement( HtmlBindingContext context, IHtmlElement element );

  }
}
