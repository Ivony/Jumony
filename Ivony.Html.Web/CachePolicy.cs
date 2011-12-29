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

    bool ResolveClientCache();

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
    /// 创建缓存项
    /// </summary>
    /// <returns></returns>
    public abstract CacheItem CreateCacheItem( ICachedResponse cachedResponse );



    private CacheItem _cacheItem;

    /// <summary>
    /// 获取缓存项
    /// </summary>
    /// <returns></returns>
    public abstract CacheItem GetCacheItem();


    /// <summary>
    /// 创建缓存项并插入缓存
    /// </summary>
    /// <param name="cachedResponse"></param>
    /// <returns></returns>
    public virtual CacheItem InsertToCache( ICachedResponse cachedResponse )
    {
      var cacheItem = CreateCacheItem( cachedResponse );
      HttpContext.Cache.InsertCacheItem( cacheItem );
      return cacheItem;
    }
  }

}
