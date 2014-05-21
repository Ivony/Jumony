using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义特定元素数据绑定器，对特定的元素直接接管数据绑定。
  /// </summary>
  public interface IHtmlElementBinder
  {

    /// <summary>
    /// 要进行处理的元素的名称
    /// </summary>
    string ElementName { get; }

    /// <summary>
    /// 接管元素的数据绑定
    /// </summary>
    /// <param name="context">当前数据绑定上下文</param>
    /// <param name="element">当前正在执行数据绑定的元素</param>
    void BindElement( HtmlBindingContext context, IHtmlElement element );
  }
}
