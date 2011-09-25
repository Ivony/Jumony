using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 定义缓存项，缓存项包括缓存的响应和策略。
  /// </summary>
  public sealed class CacheItem
  {


    /// <summary>
    /// 创建一个缓存项
    /// </summary>
    /// <param name="provider">创建该缓存项的提供程序</param>
    /// <param name="token">缓存项的缓存依据</param>
    /// <param name="cached">缓存的数据</param>
    /// <param name="duration">最大缓存时间</param>
    public CacheItem( IHtmlCachePolicyProvider provider, CacheToken token, ICachedResponse cached, TimeSpan duration )
    {
      CacheToken = token;
      CachedResponse = cached;
      Provider = provider;
      Expiration = DateTime.UtcNow + duration;
      DurationFromCreated = duration;
    }



    /// <summary>
    /// 创建缓存项的缓存策略提供程序
    /// </summary>
    public IHtmlCachePolicyProvider Provider
    {
      get;
      private set;
    }



    /// <summary>
    /// 缓存项的依据
    /// </summary>
    public CacheToken CacheToken
    {
      get;
      private set;
    }


    /// <summary>
    /// 缓存的响应数据
    /// </summary>
    public ICachedResponse CachedResponse
    {
      get;
      private set;
    }


    /// <summary>
    /// 缓存依赖项
    /// </summary>
    public CacheDependency Dependency
    {
      get;
      private set;
    }


    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public DateTime Expiration
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建缓存项时设置的缓存持续时间
    /// </summary>
    protected TimeSpan DurationFromCreated
    {
      get;
      private set;
    }




    /// <summary>
    /// 最大可能的偏移量
    /// </summary>
    protected static readonly TimeSpan maxShake =  TimeSpan.FromMinutes( 3 );


    /// <summary>
    /// 根据缓存项的设置，设置客户端的 maxage 缓存策略
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cachePolicy"></param>
    public void SetMaxAge( HttpCachePolicyBase cachePolicy )
    {
      var shake = Math.Min( DurationFromCreated.TotalMilliseconds / 50, maxShake.TotalMilliseconds );
      SetMaxAge( cachePolicy, TimeSpan.FromMilliseconds( shake ) );
    }


    /// <summary>
    /// 根据缓存项的设置，设置客户端的 maxage 缓存策略
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cachePolicy"></param>
    public void SetMaxAge( HttpCachePolicyBase cachePolicy, TimeSpan shake )
    {
      var random = new Random( DateTime.Now.Millisecond );

      var offset = TimeSpan.FromMilliseconds( random.NextDouble() * shake.TotalMilliseconds );

      var age = Expiration - DateTime.UtcNow + offset;
      cachePolicy.SetMaxAge( age );


    }



    /// <summary>
    /// 尝试设置 ETag 标签
    /// </summary>
    /// <param name="cachePolicy"></param>
    public bool TrySetETag( HttpCachePolicyBase cachePolicy )
    {
      if ( ETag == null )
        return false;

      cachePolicy.SetETag( ETag );
      return true;
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    /// <param name="cachePolicy"></param>
    public void ApplyClientCachePolicy( HttpCachePolicyBase cachePolicy )
    {
      cachePolicy.SetCacheability( HttpCacheability.Public );

      TrySetETag( cachePolicy );
      SetMaxAge( cachePolicy );
    }



    private bool _etagCreated = false;
    private string _etag;

    /// <summary>
    /// 获取缓存项的 ETag 标识
    /// </summary>
    public string ETag
    {
      get
      {
        if ( _etagCreated )
          return _etag;

        var clientCachable = CachedResponse as IClientCacheableResponse;

        if ( clientCachable == null )
          _etag = null;

        else
          _etag = clientCachable.CreateETag();

        _etagCreated = true;
        return _etag;
      }
    }
  }


  public static class CacheExtensions
  {

    /// <summary>
    /// 插入一个缓存项到 WebCache
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="item"></param>
    public static void InsertCacheItem( this Cache cache, CacheItem item )
    {
      cache.Insert( item.CacheToken.CacheKey(), item, item.Dependency, item.Expiration, Cache.NoSlidingExpiration );
    }


    /// <summary>
    /// 从 WebCache 获取一个缓存项
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static CacheItem GetCacheItem( this Cache cache, CacheToken token )
    {
      return cache.Get( token.CacheKey() ) as CacheItem;
    }


    /// <summary>
    /// 从 WebCache 获取缓存的响应
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static ICachedResponse GetCachedResponse( this Cache cache, CacheToken token )
    {
      var cacheItem = cache.GetCacheItem( token );
      if ( cacheItem == null )
        return null;

      else
        return cacheItem.CachedResponse;
    }




  }
}
