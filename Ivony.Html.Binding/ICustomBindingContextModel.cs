using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 自定义 BindingContext 的 DataModel 需要实现的接口
  /// </summary>
  public interface ICustomBindingContextModel
  {

    /// <summary>
    /// 创建 HtmlBindingContext 对象
    /// </summary>
    /// <param name="context">当前的数据绑定上下文</param>
    /// <param name="scope">新的数据绑定上下文的范畴</param>
    /// <returns></returns>
    HtmlBindingContext CreateBindingContext( HtmlBindingContext context, IHtmlContainer scopel );

  }
}
