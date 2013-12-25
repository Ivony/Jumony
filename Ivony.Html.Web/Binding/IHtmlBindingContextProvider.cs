using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 绑定上下文提供程序
  /// </summary>
  public interface IHtmlBindingContextProvider
  {

    /// <summary>
    /// 创建 HTML 绑定上下文
    /// </summary>
    /// <param name="bindingContext">当前绑定上下文（即父级上下文）</param>
    /// <param name="scope">要创建绑定上下文的 HTML 范围</param>
    /// <param name="dataContext">要创建绑定上下文的数据上下文</param>
    /// <param name="dataValues">要创建绑定上下文的扩展数据</param>
    /// <returns>绑定上下文</returns>
    HtmlBindingContext CreateBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues );

  }
}
