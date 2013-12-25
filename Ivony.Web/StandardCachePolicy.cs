using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
{



  /// <summary>
  /// 标准缓存策略
  /// </summary>
  public class StandardCachePolicy : CachePolicy, IClientCacheablePolicy
  {



    /// <summary>
    /// 创建一个标准缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="token">缓存标示</param>
    /// <param name="provider">缓存策略提供程序</param>
    /// <param name="duration">缓存持续时间</param>
    /// <param name="enableClientCache">是否启用客户端缓存</param>
    public StandardCachePolicy( HttpContextBase context, CacheToken token, ICachePolicyProvider provider, TimeSpan duration, bool enableClientCache )
      : this( context, token, provider, duration, enableClientCache, new WebCacheStorageProvider() )
    {

    }


    /// <summary>
    /// 创建一个标准缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="token">缓存标示</param>
    /// <param name="provider">缓存策略提供程序</param>
    /// <param name="duration">缓存持续时间</param>
    /// <param name="enableClientCache">是否启用客户端缓存</param>
    /// <param name="localcacheVirtualPath">静态文件缓存虚拟路径</param>
    /// <param name="enableMemoryCache">是否启用内存缓存</param>
    public StandardCachePolicy( HttpContextBase context, CacheToken token, ICachePolicyProvider provider, TimeSpan duration, bool enableClientCache, string localcacheVirtualPath, bool enableMemoryCache )
      : base( context, token, provider )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( provider == null )
        throw new ArgumentNullException( "provider" );


      Duration = duration;
      EnableClientCache = enableClientCache;

      if ( localcacheVirtualPath != null )
      {
        var physicalPath = context.Server.MapPath( localcacheVirtualPath );
        CacheStorageProvider = new StaticFileCacheStorageProvider( physicalPath, enableMemoryCache );
      }
      else
      {
        CacheStorageProvider = new WebCacheStorageProvider( HttpRuntime.Cache );
      }

    }


    /// <summary>
    /// 创建一个标准缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="token">缓存标示</param>
    /// <param name="provider">缓存策略提供程序</param>
    /// <param name="duration">缓存持续时间</param>
    /// <param name="enableClientCache">是否启用客户端缓存</param>
    /// <param name="storageProvider">缓存储存提供程序</param>
    public StandardCachePolicy( HttpContextBase context, CacheToken token, ICachePolicyProvider provider, TimeSpan duration, bool enableClientCache, ICacheStorageProvider storageProvider )
      : base( context, token, provider )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( provider == null )
        throw new ArgumentNullException( "provider" );


      Duration = duration;
      EnableClientCache = enableClientCache;

      CacheStorageProvider = storageProvider;
    }




    /// <summary>
    /// 缓存项储存提供程序
    /// </summary>
    protected ICacheStorageProvider CacheStorageProvider
    {
      get;
      set;
    }


    /// <summary>
    /// 尝试输出客户端缓存
    /// </summary>
    /// <returns>是否成功</returns>
    public virtual ICachedResponse ResolveClientCache()
    {
      if ( !EnableClientCache )
        return null;

      var cacheItem = GetCacheItem();
      if ( cacheItem == null )
        return null;

      RawResponse response;
      if ( CacheHelper.IsNotModified( HttpContext, cacheItem.ETag, out response ) )
        return response;

      else
        return null;
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    public virtual void ApplyClientCachePolicy()
    {

      var clientCachePolicy = HttpContext.GetClientCachePolicy();

      if ( !EnableClientCache )
        return;

      clientCachePolicy.SetCacheability( HttpCacheability.Public );

      var cacheItem = GetCacheItem();

      if ( cacheItem != null )
        cacheItem.ApplyClientCachePolicy( clientCachePolicy );

      else
        clientCachePolicy.SetMaxAge( Duration );

      clientCachePolicy.SetVary( CacheToken.VaryHeaders );
    }


    /// <summary>
    /// 创建缓存项
    /// </summary>
    /// <param name="cachedResponse"></param>
    /// <returns></returns>
    public CacheItem CreateCacheItem( ICachedResponse cachedResponse )
    {
      return new CacheItem( CacheToken, cachedResponse, Duration );
    }


    /// <summary>
    /// 缓存持续时间
    /// </summary>
    public TimeSpan Duration
    {
      get;
      private set;
    }

    /// <summary>
    /// 是否启用客户端缓存
    /// </summary>
    public bool EnableClientCache
    {
      get;
      private set;
    }

    /// <summary>
    /// 本地静态缓存路径
    /// </summary>
    public string LocalCacheVirtualPath
    {
      get;
      private set;
    }

    /// <summary>
    /// 是否启用内存缓存
    /// </summary>
    public bool EnableMemoryCache
    {
      get;
      private set;
    }


    private CacheItem _cacheItem;

    /// <summary>
    /// 获取 CacheItem
    /// </summary>
    /// <returns></returns>
    public CacheItem GetCacheItem()
    {

      if ( _cacheItem == null )
        _cacheItem = CacheStorageProvider.GetCacheItem( CacheToken );

      if ( _cacheItem != null && _cacheItem.IsValid() )//确保缓存未过期
        return _cacheItem;

      return null;


    }


    /// <summary>
    /// 尝试获取缓存的输出
    /// </summary>
    /// <returns>可用的已被缓存的输出</returns>
    public override ICachedResponse ResolveCache()
    {
      var cacheItem = GetCacheItem();
      if ( cacheItem == null )
        return null;

      return cacheItem.CachedResponse;
    }


    /// <summary>
    /// 插入缓存
    /// </summary>
    /// <param name="cachedResponse">可被缓存的响应数据</param>
    /// <returns>缓存项</returns>
    public override CacheItem UpdateCache( ICachedResponse cachedResponse )
    {

      var cacheItem = CreateCacheItem( cachedResponse );

      CacheStorageProvider.InsertCacheItem( cacheItem );

      return cacheItem;

    }



  }
}
