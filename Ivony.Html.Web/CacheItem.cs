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

    public CacheItem( IHtmlCachePolicyProvider provider, CacheToken token, ICachedResponse cached, TimeSpan duration )
    {
      CacheToken = token;
      CachedResponse = cached;
      Provider = provider;
      Expiration = DateTime.UtcNow + duration;
    }


    public IHtmlCachePolicyProvider Provider
    {
      get;
      private set;
    }



    public CacheToken CacheToken
    {
      get;
      private set;
    }

    public ICachedResponse CachedResponse
    {
      get;
      private set;
    }

    public CacheDependency Dependency
    {
      get;
      private set;
    }

    public DateTime Expiration
    {
      get;
      private set;
    }



  }


  public static class CacheExtensions
  {
    public static void InsertCacheItem( this Cache cache, CacheItem item )
    {
      cache.Insert( item.CacheToken.CacheKey(), item, item.Dependency, item.Expiration, Cache.NoSlidingExpiration );
    }

    public static CacheItem GetCacheItem( this Cache cache, CacheToken token )
    {
      return cache.Get( token.CacheKey() ) as CacheItem;
    }

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
