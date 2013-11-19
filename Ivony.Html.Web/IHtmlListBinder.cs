using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 代表一个列表数据绑定器
  /// </summary>
  public interface IHtmlListBinder
  {

    /// <summary>
    /// 绑定元素
    /// </summary>
    /// <param name="element">要绑定的元素</param>
    /// <param name="context">数据绑定上下文</param>
    /// <returns>是否执行了绑定，若执行了绑定，则对于这个元素其他绑定器不再运行</returns>
    bool BindList( IHtmlElement element, HtmlListBindingContext bindingContext );


  }
}
