using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Ivony.Fluent;

namespace Ivony.Web
{


  /// <summary>
  /// 定义缓存策略提供程序
  /// </summary>
  public interface ICachePolicyProvider
  {

    /// <summary>
    /// 为当前请求创建缓存策略
    /// </summary>
    /// <param name="context">当前 HTTP 请求</param>
    /// <returns>当前请求的缓存策略</returns>
    CachePolicy CreateCachePolicy( HttpContextBase context );

  }


  /// <summary>
  /// 用于枚举设置默认缓存键策略（缓存依据）
  /// </summary>
  [Flags]
  public enum CacheKeyPolicy
  {

    /// <summary>指定对于所有请求都不要缓存，即不产生任何CacheKey</summary>
    NoCache = 0,

    /// <summary>指定以请求的虚拟路径作为缓存依据</summary>
    ByVirtualPath = 1,

    /// <summary>指定以SessionID作为依据</summary>
    BySession = 2,

    /// <summary>指定以Identity的Name作为依据</summary>
    ByIdentity = 4,
  }




}
