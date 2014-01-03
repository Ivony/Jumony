using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 定义提供 HTML 元素绑定器的提供程序
  /// </summary>
  public interface IHtmlBinderProvider
  {

    IHtmlBinder[] GetHtmlBinders();

    IExpressionBinder[] GetExpressionBinders();

  }
}
