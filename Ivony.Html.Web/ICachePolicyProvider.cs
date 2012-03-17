using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Ivony.Fluent;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 定义缓存策略提供程序
  /// </summary>
  public interface ICachePolicyProvider
  {

    /// <summary>
    /// 为当前请求创建缓存策略
    /// </summary>
    /// <param name="context">当前 HTTP 请求</param>
    /// <returns>当前请求的缓存策略</returns>
    CachePolicy CreateCachePolicy( HttpContextBase context );

  }


  /// <summary>
  /// 用于枚举设置默认缓存键策略（缓存依据）
  /// </summary>
  [Flags]
  public enum CacheKeyPolicy
  {

    /// <summary>指定对于所有请求都不要缓存，即不产生任何CacheKey</summary>
    NoCache = 0,

    /// <summary>指定以请求的虚拟路径作为缓存依据</summary>
    ByVirtualPath = 1,

    /// <summary>指定以SessionID作为依据</summary>
    BySession = 2,

    /// <summary>指定以Identity的Name作为依据</summary>
    ByIdentity = 4,
  }


  /// <summary>
  /// 用于设置和获取默认缓存策略
  /// </summary>
  public class DefaultCachePolicyProvider : ICachePolicyProvider
  {


    /// <summary>
    /// 获取默认缓存策略提供程序的实例
    /// </summary>
    public static DefaultCachePolicyProvider Instance
    {
      get;
      private set;
    }


    static DefaultCachePolicyProvider()
    {
      CacheKeyPolicy = CacheKeyPolicy.NoCache;
      CacheDuration = new TimeSpan( 0 );
      Instance = new DefaultCachePolicyProvider();
    }

    private DefaultCachePolicyProvider() { }


    /// <summary>
    /// 设置默认缓存键的产生依据
    /// </summary>
    public static CacheKeyPolicy CacheKeyPolicy
    {
      get;
      set;
    }


    /// <summary>
    /// 获取或设置默认缓存时间
    /// </summary>
    public static TimeSpan CacheDuration
    {
      get;
      set;
    }


    /// <summary>
    /// 按照设置的默认规则获取缓存标记
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <returns></returns>
    public static CacheToken GetCacheToken( HttpContextBase context )
    {
      if ( StringComparer.OrdinalIgnoreCase.Equals( "post", context.Request.HttpMethod ) )
        return null;

      if ( CacheKeyPolicy == Web.CacheKeyPolicy.NoCache )
        return null;

      CacheToken token = null;

      if ( (CacheKeyPolicy & Web.CacheKeyPolicy.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          token += CacheToken.FromSessionID( context );
      }

      if ( (CacheKeyPolicy & Web.CacheKeyPolicy.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          token += CacheToken.FromCookies( context );
      }

      if ( (CacheKeyPolicy & Web.CacheKeyPolicy.ByVirtualPath) != 0 )
        token += CacheToken.FromVirtualPath( context );

      return token;
    }


    /// <summary>
    /// 获取默认缓存策略
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public CachePolicy CreateCachePolicy( HttpContextBase context )
    {
      var token = GetCacheToken( context );
      if ( token == null )
        return null;

      return new StandardCachePolicy( context, token, this, CacheDuration, true );
    }
  }


}
