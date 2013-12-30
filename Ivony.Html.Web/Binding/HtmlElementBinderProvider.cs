using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// HTML 绑定器提供程序
  /// </summary>
  public static class HtmlElementBinderProvider
  {

    /// <summary>
    /// 获取当前可用的所有元素绑定器
    /// </summary>
    /// <param name="handler">当前 HTML 页面处理程序</param>
    /// <returns>所有可用的元素绑定器</returns>
    public static IHtmlBinder[] GetBinders( object handler )
    {
      return new[] { new StyleBinder() };
    }


  }
}
