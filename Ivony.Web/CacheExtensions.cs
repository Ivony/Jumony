using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Compilation;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivony.Web
{
  
  /// <summary>
  /// 包含 WebCache 一些扩展方法
  /// </summary>
  public static class CacheExtensions
  {


    /// <summary>
    /// 获取当前 HTTP 请求的客户端缓存策略
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ClientCachePolicyBase GetClientCachePolicy( this HttpContextBase context )
    {
      var instance = context.Items[ClientCachePolicy.Token] as ClientCachePolicy;

      if ( instance == null )
        return new ClientCachePolicyWrapper( context.Response.Cache );

      else
        return instance;
    }


    
    /// <summary>
    /// 向 System.Web.Caching.Cache 对象中插入对象，使用指定的优先级策略。
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key">用于引用该对象的缓存键。</param>
    /// <param name="value">要插入缓存中的对象。</param>
    /// <param name="priority">该对象相对于缓存中存储的其他项的成本，由 System.Web.Caching.CacheItemPriority 枚举表示。该值由缓存在退出对象时使用；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
    public static void Insert( this Cache cache, string key, object value, CacheItemPriority priority )
    {
      cache.Insert( key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, null );
    }


    /// <summary>
    /// 向 System.Web.Caching.Cache 对象中插入对象，使用指定的过期时间。
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key">用于引用该对象的缓存键。</param>
    /// <param name="value">要插入缓存中的对象。</param>
    /// <param name="expiration">所插入对象将到期并被从缓存中移除的时间。</param>
    public static void Insert( this Cache cache, string key, object value, DateTimeOffset expiration )
    {
      cache.Insert( key, value, null, expiration.UtcDateTime, Cache.NoSlidingExpiration );
    }

    
    /// <summary>
    /// 向 System.Web.Caching.Cache 对象中插入对象，使用指定的过期时间。
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key">用于引用该对象的缓存键。</param>
    /// <param name="value">要插入缓存中的对象。</param>
    /// <param name="dependency">该项的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。</param>
    /// <param name="priority">该对象相对于缓存中存储的其他项的成本，由 System.Web.Caching.CacheItemPriority 枚举表示。该值由缓存在退出对象时使用；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
    public static void Insert( this Cache cache, string key, object value, CacheDependency dependency, CacheItemPriority priority )
    {
      cache.Insert( key, value, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, null );
    }






    /// <summary>
    /// 插入一个缓存项到 WebCache
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="item"></param>
    public static void InsertCacheItem( this Cache cache, CacheItem item )
    {
      cache.Insert( item.CacheToken.CacheKey(), item, null, item.Expiration, Cache.NoSlidingExpiration );
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

#if false

    /// <summary>
    /// 将 CacheItem 对象序列化到指定文件路径
    /// </summary>
    /// <param name="cacheItem">要序列化的 CacheItem 对象</param>
    /// <param name="filepath">序列化的位置</param>
    public static void SerializeTo( this CacheItem cacheItem, string filepath )
    {
      Directory.CreateDirectory( Path.GetDirectoryName( filepath ) );
      using ( var stream = File.OpenWrite( filepath ) )
      {
        SerializeTo( cacheItem, stream );
      }
    }


    /// <summary>
    /// 将 CacheItem 对象序列化到指定的流
    /// </summary>
    /// <param name="cacheItem">要序列化的 CacheItem 对象</param>
    /// <param name="stream">序列化的流</param>
    public static void SerializeTo( this CacheItem cacheItem, Stream stream )
    {

      var fomatter = new BinaryFormatter();
      fomatter.Serialize( stream, cacheItem );

    }



    /// <summary>
    /// 将 CacheItem 对象从指定文件路径反序列化还原
    /// </summary>
    /// <param name="provider">创建此 CacheItem 对象的 ICachePolicyProvider</param>
    /// <param name="filepath">文件路径</param>
    public static CacheItem DeserializeFrom( this ICachePolicyProvider provider, string filepath )
    {
      if ( !File.Exists( filepath ) )
        return null;

      using ( var stream = File.OpenRead( filepath ) )
      {
        return DeserializeFrom( provider, stream );
      }
    }


    /// <summary>
    /// 将 CacheItem 对象从指定流反序列化还原
    /// </summary>
    /// <param name="provider">创建此 CacheItem 对象的 ICachePolicyProvider</param>
    /// <param name="stream">用于反序列化的读取流</param>
    public static CacheItem DeserializeFrom( this ICachePolicyProvider provider, Stream stream )
    {

      var fomatter = new BinaryFormatter();

      CacheItem item;
      item = (CacheItem) fomatter.Deserialize( stream );

      if ( item.Expiration < DateTime.UtcNow )//缓存已过期
        return null;

      item.Provider = provider;
      return item;
    }

#endif


    private static readonly Regex invalidPathCharactor = new Regex( @"\W+", RegexOptions.Compiled );



  }
}
