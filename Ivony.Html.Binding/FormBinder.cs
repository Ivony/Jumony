using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 实现进行表单绑定的绑定器
  /// </summary>
  public class FormBinder : IHtmlBinder
  {

    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="element">要绑定的元素</param>
    /// <returns>是否应当继续处理后面的绑定器</returns>
    public bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      throw new NotImplementedException();
    }
  }
}
