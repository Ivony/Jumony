using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 定义可被缓存的 ActionResult
  /// </summary>
  public interface ICachableResult
  {

    /// <summary>
    /// 获取用于缓存的 ActionResult ，一般是 ContentResult 的实例。
    /// </summary>
    ActionResult CachedResult
    {
      get;
    }

  }
}
