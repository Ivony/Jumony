using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  
  
  /// <summary>
  /// 定义 MVC 上下文相关的缓存策略
  /// </summary>
  public interface IMvcCachePolicy
  {

    /// <summary>
    /// 创建缓存项
    /// </summary>
    /// <param name="context">当前 MVC 上下文</param>
    /// <param name="result">要缓存的执行结果</param>
    /// <returns></returns>
    void UpdateCache( ControllerContext context, ActionResult result );

  }
}
