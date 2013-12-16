using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  /// <summary>
  /// 实现此接口以实现客户端缓存策略
  /// </summary>
  public interface IClientCacheablePolicy
  {

    /// <summary>
    /// 尝试输出客户端缓存
    /// </summary>
    /// <returns></returns>
    ICachedResponse ResolveClientCache();


    /// <summary>
    /// 尝试应用客户端缓存策略
    /// </summary>
    void ApplyClientCachePolicy();

  }


}
