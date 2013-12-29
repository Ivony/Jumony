using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义提供 HTML 元素绑定器的提供程序
  /// </summary>
  public interface IHtmlBinderProvider
  {

    IHtmlElementBinder[] GetBinders();

  }
}
