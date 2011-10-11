using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
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
      : this( context, token, provider, duration, enableClientCache, null, true ) { }


    /// <summary>
    /// 创建一个标准缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="token">缓存标示</param>
    /// <param name="provider">缓存策略提供程序</param>
    /// <param name="duration">缓存持续时间</param>
    /// <param name="enableClientCache">是否启用客户端缓存</param>
    public StandardCachePolicy( HttpContextBase context, CacheToken token, ICachePolicyProvider provider, TimeSpan duration, bool enableClientCache, string localcacheVirtualPath, bool enableMemoryCache )
      : base( context, token, provider )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( provider == null )
        throw new ArgumentNullException( "provider" );


      Duration = duration;
      EnableClientCache = enableClientCache;
      LocalCacheVirtualPath = VirtualPathUtility.AppendTrailingSlash( localcacheVirtualPath );
      EnableMemoryCache = enableMemoryCache;
    }



    /// <summary>
    /// 尝试输出客户端缓存
    /// </summary>
    /// <param name="context">当前请求上下文</param>
    /// <returns>是否成功</returns>
    public virtual bool ResolveClientCache()
    {
      if ( !EnableClientCache )
        return false;

      var cacheItem = GetCacheItem();
      if ( cacheItem == null )
        return false;

      return CacheHelper.IsNotModified( HttpContext, cacheItem.ETag );
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    /// <param name="clientCachePolicy">客户端缓存策略</param>
    public virtual void ApplyClientCachePolicy()
    {

      var clientCachePolicy = HttpContext.Response.Cache;

      if ( !EnableClientCache )
        return;

      clientCachePolicy.SetCacheability( HttpCacheability.Public );

      var cacheItem = GetCacheItem();

      if ( cacheItem != null )
        cacheItem.ApplyClientCachePolicy( clientCachePolicy );

      else
        clientCachePolicy.SetMaxAge( Duration );
    }


    /// <summary>
    /// 创建缓存项
    /// </summary>
    /// <param name="cachedResponse"></param>
    /// <returns></returns>
    public override CacheItem CreateCacheItem( ICachedResponse cachedResponse )
    {
      return new CacheItem( Provider, CacheToken, cachedResponse, Duration );
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
    public override CacheItem GetCacheItem()
    {

      if ( _cacheItem != null )
        return _cacheItem;

      _cacheItem = base.GetCacheItem();
      if ( _cacheItem != null || LocalCacheVirtualPath == null )
        return _cacheItem;

      var physicalPath = GetCacheFilepath( CacheToken );

      return _cacheItem = CacheExtensions.DeserializeFrom( Provider, physicalPath );
    }

    private string GetCacheFilepath( CacheToken token )
    {
      var filename = token.CreateFilename();
      var virtualPath = VirtualPathUtility.Combine( LocalCacheVirtualPath, filename );

      var physicalPath = HttpContext.Server.MapPath( virtualPath );
      return physicalPath;
    }


    /// <summary>
    /// 插入缓存
    /// </summary>
    /// <param name="cachedResponse"></param>
    /// <returns></returns>
    public override CacheItem InsertToCache( ICachedResponse cachedResponse )
    {

      var cacheItem = CreateCacheItem( cachedResponse );

      if ( LocalCacheVirtualPath == null || EnableMemoryCache )
      {
        cacheItem = base.InsertToCache( cachedResponse );
      }

      if ( LocalCacheVirtualPath != null )
      {
        if ( Duration > TimeSpan.FromMinutes( 2 ) )
          cacheItem.SerializeTo( GetCacheFilepath( cacheItem.CacheToken ) );
      }

      return cacheItem;
    }



  }
}
