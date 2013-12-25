using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 标记当前路由对象是可以分配 HTML 请求的
  /// </summary>
  /// <remarks>
  /// 这是一个标记接口，没有任何需要实现的成员，当进入 JumonyHandler 时，会自动检查当前的路由对象是否实现了这个接口，若没有实现接口，则 JumonyHandler 会认为这是非法请求。
  /// </remarks>
  public interface IHtmlRequestRoute
  {
  }
}
