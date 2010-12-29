using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  
  /// <summary>
  /// 代表一个缓存策略
  /// </summary>
  public class HtmlCachePolicy
  {

    /// <summary>
    /// 缓存依赖项
    /// </summary>
    public CacheDependency Dependency { get; set; }

    /// <summary>
    /// 缓存持续时间
    /// </summary>
    public TimeSpan Duration { get; set; }


  }
}
