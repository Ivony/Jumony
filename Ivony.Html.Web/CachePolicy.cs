using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{



  /// <summary>
  /// 实现此接口完成客户端缓存策略
  /// </summary>
  public interface IClientCachablePolicy
  {

    bool ResolveClientCache( HttpContextBase context );

    void ApplyClientCachePolicy( HttpCachePolicyBase policy );

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
    public CachePolicy( HttpContextBase context, CacheToken token, IHtmlCachePolicyProvider provider )
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
    public IHtmlCachePolicyProvider Provider
    {
      get;
      private set;
    }



    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    /// <param name="clientCachePolicy"></param>
    public virtual void ApplyClientCachePolicy( HttpCachePolicyBase clientCachePolicy )
    {

    }



    /// <summary>
    /// 创建缓存项
    /// </summary>
    /// <returns></returns>
    public abstract CacheItem CreateCacheItem( ICachedResponse cachedResponse );
  }



  /// <summary>
  /// 标准缓存策略
  /// </summary>
  public class StandardCachePolicy : CachePolicy, IClientCachablePolicy
  {
    public StandardCachePolicy( HttpContextBase context, CacheToken token, IHtmlCachePolicyProvider provider, TimeSpan duration, bool enableClientCache )
      : base( context, token, provider )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      if ( provider == null )
        throw new ArgumentNullException( "provider" );


      Duration = duration;
      EnableClientCache = enableClientCache;
    }



    public bool ResolveClientCache( HttpContextBase context )
    {
      if ( !EnableClientCache )
        return false;

      var cacheItem = HttpContext.Cache.GetCacheItem( CacheToken );
      if ( cacheItem == null )
        return false;

      return CacheHelper.IsNotModified( context, cacheItem.ETag );
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    /// <param name="clientCachePolicy"></param>
    public override void ApplyClientCachePolicy( HttpCachePolicyBase clientCachePolicy )
    {
      if ( !EnableClientCache )
        return;

      var cacheItem = HttpContext.Cache.GetCacheItem( CacheToken );

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


    public TimeSpan Duration
    {
      get;
      private set;
    }

    public bool EnableClientCache
    {
      get;
      private set;
    }

  }

}
