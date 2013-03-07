using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Ivony.Web
{

  /// <summary>
  /// 输出缓存储存提供程序
  /// </summary>
  public interface ICacheStorageProvider
  {

    /// <summary>
    /// 插入一个缓存项
    /// </summary>
    /// <param name="cacheItem">缓存项</param>
    void InsertCacheItem( CacheItem cacheItem );


    /// <summary>
    /// 获取一个缓存项
    /// </summary>
    /// <param name="token">缓存标示</param>
    /// <returns>缓存项</returns>
    CacheItem GetCacheItem( CacheToken token );

  }


  /// <summary>
  /// WebCache 的缓存储存提供程序。
  /// </summary>
  public class WebCacheStorageProvider : ICacheStorageProvider
  {

    private Cache _cache;

    /// <summary>
    /// 创建 WebCacheStorageProvider 的实例
    /// </summary>
    public WebCacheStorageProvider() : this( HttpRuntime.Cache ) { }


    /// <summary>
    /// 创建 WebCacheStorageProvider 的实例
    /// </summary>
    /// <param name="cache">WebCache 对象</param>
    public WebCacheStorageProvider( Cache cache )
    {
      _cache = cache;
    }


    /// <summary>
    /// 插入一个缓存项
    /// </summary>
    /// <param name="cacheItem">要插入的缓存项</param>
    public void InsertCacheItem( CacheItem cacheItem )
    {
      _cache.InsertCacheItem( cacheItem );
    }


    /// <summary>
    /// 获取一个缓存项
    /// </summary>
    /// <param name="token">缓存标示</param>
    /// <returns></returns>
    public CacheItem GetCacheItem( CacheToken token )
    {
      return _cache.GetCacheItem( token );
    }
  }






}
