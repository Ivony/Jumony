using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
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
    void BindElement( HtmlBindingContext context );

  }
}
