using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Text.RegularExpressions;

namespace Ivony.Web
{


  /// <summary>
  /// 定义缓存项，缓存项包括缓存的响应和策略。
  /// </summary>
  [Serializable]
  public sealed class CacheItem
  {


    /// <summary>
    /// 创建一个缓存项
    /// </summary>
    /// <param name="token">缓存项的缓存依据</param>
    /// <param name="cached">缓存的数据</param>
    /// <param name="duration">最大缓存时间</param>
    public CacheItem( CacheToken token, ICachedResponse cached, TimeSpan duration )
    {
      CacheToken = token;
      CachedResponse = cached;

      var shake = Math.Min( DurationWhenCreated.TotalMilliseconds / 50, maxShake.TotalMilliseconds );
      var random = new Random( DateTime.Now.Millisecond );
      var offset = TimeSpan.FromMilliseconds( random.NextDouble() * shake );

      Expiration = DateTime.UtcNow + duration + offset;

      DurationWhenCreated = duration;
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
    private TimeSpan DurationWhenCreated
    {
      get;
      set;
    }


    /// <summary>
    /// 缓存项是否还有效
    /// </summary>
    /// <returns>是否有效</returns>
    public bool IsValid()
    {
      if ( Expiration < DateTime.UtcNow )
        return false;

      if ( CacheToken.CacheDependency != null && CacheToken.CacheDependency.HasChanged )
        return false;

      return true;
    }

    /// <summary>
    /// 缓存项是否还有效
    /// </summary>
    /// <param name="token">缓存标识，检查检查缓存项的标识是否与提供的一致，否则也认为缓存项无效</param>
    /// <returns>是否有效</returns>
    public bool IsValid( CacheToken token )
    {
      if ( IsValid() )
        return CacheToken == token;

      return false;
    }



    /// <summary>
    /// 最大可能的偏移量
    /// </summary>
    private static readonly TimeSpan maxShake = TimeSpan.FromMinutes( 3 );


    /// <summary>
    /// 根据缓存项的设置，设置客户端的 maxage 缓存策略
    /// </summary>
    /// <param name="cachePolicy"></param>
    public void SetMaxAge( ClientCachePolicyBase cachePolicy )
    {
      var shake = Math.Min( DurationWhenCreated.TotalMilliseconds / 50, maxShake.TotalMilliseconds );
      SetMaxAge( cachePolicy, TimeSpan.FromMilliseconds( shake ) );
    }


    /// <summary>
    /// 根据缓存项的设置，设置客户端的 maxage 缓存策略
    /// </summary>
    /// <param name="cachePolicy"></param>
    /// <param name="shake"></param>
    public void SetMaxAge( ClientCachePolicyBase cachePolicy, TimeSpan shake )
    {


      var delta = Expiration - DateTime.UtcNow;

      if ( delta <= TimeSpan.Zero )
      {
        cachePolicy.SetMaxAge( TimeSpan.Zero );
        return;
      }


      var random = new Random( DateTime.Now.Millisecond );
      var offset = TimeSpan.FromMilliseconds( random.NextDouble() * shake.TotalMilliseconds );

      var age = delta + offset;
      cachePolicy.SetMaxAge( age );

    }



    /// <summary>
    /// 尝试设置 ETag 标签
    /// </summary>
    /// <param name="cachePolicy"></param>
    public bool TrySetETag( ClientCachePolicyBase cachePolicy )
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
    public void ApplyClientCachePolicy( ClientCachePolicyBase cachePolicy )
    {
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

        var clientCacheable = CachedResponse as IClientCacheableResponse;

        if ( clientCacheable == null )
          _etag = null;

        else
          _etag = clientCacheable.CreateETag();

        _etagCreated = true;
        return _etag;
      }
    }
  }


}
