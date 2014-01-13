using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
{



  /// <summary>
  /// 缓存策略
  /// </summary>
  public abstract class CachePolicy
  {

    /// <summary>
    /// 创建 CachePolicy 实例
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="token">缓存标识</param>
    /// <param name="provider">缓存策略提供程序</param>
    public CachePolicy( HttpContextBase context, CacheToken token, ICachePolicyProvider provider )
    {
      HttpContext = context;
      CacheToken = token;
      Provider = provider;
    }



    /// <summary>
    /// 获取当前 HTTP 请求
    /// </summary>
    public HttpContextBase HttpContext
    {
      get;
      private set;
    }



    /// <summary>
    /// 缓存标记
    /// </summary>
    public CacheToken CacheToken
    {
      get;
      private set;
    }


    /// <summary>
    /// 缓存策略提供程序
    /// </summary>
    public ICachePolicyProvider Provider
    {
      get;
      private set;
    }




    /// <summary>
    /// 尝试缓存输出
    /// </summary>
    /// <returns></returns>
    public abstract ICachedResponse ResolveCache();


    /// <summary>
    /// 刷新缓存
    /// </summary>
    /// <param name="cachedResponse">可被缓存的响应</param>
    /// <returns>缓存项</returns>
    public abstract CacheItem UpdateCache( ICachedResponse cachedResponse );
  }



}
