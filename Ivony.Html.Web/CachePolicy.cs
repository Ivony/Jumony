using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
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
    bool ResolveClientCache();


    /// <summary>
    /// 尝试应用客户端缓存策略
    /// </summary>
    void ApplyClientCachePolicy();

  }


  /// <summary>
  /// 缓存策略
  /// </summary>
  public abstract class CachePolicy
  {



    /// <summary>
    /// 创建 CachePolicy 实例
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    /// <param name="provider"></param>
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
