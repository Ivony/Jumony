using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace Ivony.Web
{

  /// <summary>
  /// 持久化缓存提供程序
  /// </summary>
  public abstract class PersistentCacheStorageProvider : ICacheStorageProvider
  {
    /// <summary>
    /// 插入缓存项
    /// </summary>
    /// <param name="cacheItem"></param>
    public virtual void InsertCacheItem( CacheItem cacheItem )
    {

      if ( !cacheItem.IsValid() )//缓存已过期
        return;

      SaveCacheItem( cacheItem );
    }

    /// <summary>
    /// 获取缓存项
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual CacheItem GetCacheItem( CacheToken token )
    {

      var cacheItem = LoadCacheItem( token );

      if ( cacheItem != null && cacheItem.IsValid( token ) )//检查缓存是否已过期
        return cacheItem;

      return null;

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

        if ( stream == null )
          return null;

        var cacheItem = Deserialize( stream );

        if ( cacheItem != null && cacheItem.IsValid( token ) )
          return cacheItem;
        else
          return null;
      }
    }


    /// <summary>
    /// 序列化缓存项
    /// </summary>
    /// <param name="cacheItem"></param>
    /// <param name="stream"></param>
    protected virtual void Serialize( CacheItem cacheItem, Stream stream )
    {
      var formatter = new BinaryFormatter();
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

      if ( stream == null )
        return null;

      try
      {
        var formatter = new BinaryFormatter();
        var cacheItem = formatter.Deserialize( stream ) as CacheItem;
        if ( cacheItem == null )
          return null;

        return cacheItem;

      }
      catch
      {
        return null;
      }
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



  /// <summary>
  /// 静态文件缓存提供程序
  /// </summary>
  public class StaticFileCacheStorageProvider : PersistentCacheStorageProvider
  {

    /// <summary>
    /// 创建静态文件缓存储存提供程序
    /// </summary>
    /// <param name="physicalPath">静态缓存储存的物理路径位置</param>
    /// <param name="enableMemoryCache">是否同时启用基于内存的 WebCache 缓存</param>
    public StaticFileCacheStorageProvider( string physicalPath, bool enableMemoryCache )
    {
      PhysicalPath = physicalPath;
      EnableMemoryCache = enableMemoryCache;
    }


    /// <summary>
    /// 静态文件路径
    /// </summary>
    public string PhysicalPath
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否使用内存缓存
    /// </summary>
    public bool EnableMemoryCache
    {
      get;
      private set;
    }


    /// <summary>
    /// 插入缓存项
    /// </summary>
    /// <param name="cacheItem"></param>
    public override void InsertCacheItem( CacheItem cacheItem )
    {
      if ( EnableMemoryCache )
        HttpRuntime.Cache.InsertCacheItem( cacheItem );

      base.InsertCacheItem( cacheItem );
    }


    /// <summary>
    /// 获取缓存项
    /// </summary>
    /// <param name="token">缓存标识</param>
    /// <returns></returns>
    public override CacheItem GetCacheItem( CacheToken token )
    {
      if ( EnableMemoryCache )
      {
        var cacheItem = HttpRuntime.Cache.GetCacheItem( token );

        if ( cacheItem != null )
        {
          if ( !cacheItem.IsValid() )//缓存已过期
            return null;

          return cacheItem;
        }
      }


      return base.GetCacheItem( token );
    }



    /// <summary>
    /// 创建持久化输入流
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override Stream CreateLoadStream( CacheToken token )
    {
      var path = Path.Combine( PhysicalPath, CreateFilename( token ) );
      if ( File.Exists( path ) )
        return File.OpenRead( path );
      else
        return null;
    }

    /// <summary>
    /// 创建持久化输出流
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected override Stream CreateSaveStream( CacheToken token )
    {
      Directory.CreateDirectory( PhysicalPath );
      var path = Path.Combine( PhysicalPath, CreateFilename( token ) );
      return File.OpenWrite( path );
    }


    /// <summary>
    /// 匹配非文件名组成字符的正则表达式
    /// </summary>
    protected static readonly Regex invalidPathCharactor = new Regex( @"\W+", RegexOptions.Compiled );

    /// <summary>
    /// 根据缓存标识创建静态缓存的文件名
    /// </summary>
    /// <param name="token">缓存标识</param>
    /// <returns></returns>
    protected virtual string CreateFilename( CacheToken token )
    {
      var cacheKey = token.CacheKey();
      var name = invalidPathCharactor.Replace( token.CacheKey(), "" );

      if ( name.Length > 20 )
        name = name.Substring( 0, 20 );

      var hash = FormsAuthentication.HashPasswordForStoringInConfigFile( cacheKey, "SHA1" );

      return name + "_" + hash + ".cache";
    }


  }

}
