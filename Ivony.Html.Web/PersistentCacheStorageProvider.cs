using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace Ivony.Html.Web
{
  public abstract class PersistentCacheStorageProvider : ICacheStorageProvider
  {
    /// <summary>
    /// 插入缓存项
    /// </summary>
    /// <param name="cacheItem"></param>
    public virtual void InsertCacheItem( CacheItem cacheItem )
    {

      HttpRuntime.Cache.InsertCacheItem( cacheItem );

      SaveCacheItem( cacheItem );
    }

    /// <summary>
    /// 获取缓存项
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual CacheItem GetCacheItem( CacheToken token )
    {

      var cacheItem = HttpRuntime.Cache.GetCacheItem( token );
      if ( cacheItem != null )
        return cacheItem;

      return LoadCacheItem( token );

    }


    /// <summary>
    /// 将缓存项持久化到设备
    /// </summary>
    /// <param name="cacheItem"></param>
    protected virtual void SaveCacheItem( CacheItem cacheItem )
    {
      using ( var stream = CreateSaveStream( cacheItem.CacheToken ) )
      {
        Serialize( cacheItem, stream );
      }
    }

    /// <summary>
    /// 从持久化设备中加载缓存项
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected virtual CacheItem LoadCacheItem( CacheToken token )
    {
      using ( var stream = CreateLoadStream( token ) )
      {
        return Deserialize( stream );
      }
    }


    /// <summary>
    /// 序列化缓存项
    /// </summary>
    /// <param name="cacheItem"></param>
    /// <param name="stream"></param>
    protected virtual void Serialize( CacheItem cacheItem, Stream stream )
    {
      var formatter  = new BinaryFormatter();
      formatter.Serialize( stream, cacheItem );
    }

    /// <summary>
    /// 反序列化缓存项
    /// </summary>
    /// <param name="stream"></param>
    /// 
    /// <returns></returns>
    protected virtual CacheItem Deserialize( Stream stream )
    {
      var formatter = new BinaryFormatter();
      var cacheItem = formatter.Deserialize( stream ) as CacheItem;
      if ( cacheItem == null )
        return null;

      return cacheItem;
    }


    /// <summary>
    /// 创建持久化输出流
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Stream CreateSaveStream( CacheToken token );

    /// <summary>
    /// 创建持久化输入流
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract Stream CreateLoadStream( CacheToken token );

  }


  public class StaticFileCacheStorageProvider : PersistentCacheStorageProvider
  {
    public StaticFileCacheStorageProvider( string physicalPath, bool useMemoryCache )
    {
      PhysicalPath = physicalPath;
      UseMemoryCache = useMemoryCache;
    }

    public string PhysicalPath
    {
      get;
      private set;
    }

    public bool UseMemoryCache
    {
      get;
      private set;
    }



    public override void InsertCacheItem( CacheItem cacheItem )
    {
      if ( UseMemoryCache )
        HttpRuntime.Cache.InsertCacheItem( cacheItem );

      SaveCacheItem( cacheItem );
    }


    protected override Stream CreateLoadStream( CacheToken token )
    {
      var path = Path.Combine( PhysicalPath, token.CreateFilename() );
      return File.OpenRead( path );
    }

    protected override Stream CreateSaveStream( CacheToken token )
    {
      var path = Path.Combine( PhysicalPath, token.CreateFilename() );
      return File.OpenWrite( path );
    }

  }

}
