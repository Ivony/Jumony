using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public static class HtmlElementBinderProvider
  {

    /// <summary>
    /// 获取当前可用的所有元素绑定器
    /// </summary>
    /// <param name="handler">当前 HTML 页面处理程序</param>
    /// <returns>所有可用的元素绑定器</returns>
    public static IHtmlElementBinder[] GetBinders( object handler )
    {
      return new[] { new DefaultElementBinder() };
    }


    /// <summary>
    /// 创建默认的绑定上下文
    /// </summary>
    /// <param name="handler">当前 HTML 页面处理程序</param>
    /// <returns>当前默认的绑定上下文</returns>
    public static HtmlBindingContext CreateBindingContext( object handler )
    {
      return new HtmlBindingContext( GetBinders( handler ) );
    }

  }
}
