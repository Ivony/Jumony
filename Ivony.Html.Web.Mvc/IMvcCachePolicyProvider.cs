using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Web;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// MVC 环境的缓存策略提供程序所需要实现的接口
  /// </summary>
  public interface IMvcCachePolicyProvider : ICachePolicyProvider
  {

    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="action">Action 信息</param>
    /// <param name="parameters">Action 参数</param>
    /// <returns>缓存策略</returns>
    CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters );

  }



}
