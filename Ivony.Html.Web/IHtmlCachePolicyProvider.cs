using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 定义缓存策略提供程序
  /// </summary>
  public interface IHtmlCachePolicyProvider
  {

    string GetCacheKey( HttpContextBase context );

    HtmlCachePolicy GetPolicy( HttpContextBase context, IHtmlHandler handler, RawResponse cacheItem );

  }


  /// <summary>
  /// 用于枚举设置默认缓存键策略（缓存依据）
  /// </summary>
  [Flags]
  public enum CacheKeyProlicy
  {

    /// <summary>指定对于所有请求都不要缓存，即不产生任何CacheKey</summary>
    NoCache = 0,

    /// <summary>指定对于所有请求，以url作为缓存依据</summary>
    ByUrl = 1,

    /// <summary>指定对于所有请求，以SessionID作为依据</summary>
    BySession = 2,

    /// <summary>指定对于所有请求，以Identity的Name作为依据</summary>
    ByIdentity = 4,


  }


  /// <summary>
  /// 用于设置和获取默认缓存策略
  /// </summary>
  public static class DefaultCachePolicy
  {


    static DefaultCachePolicy()
    {
      CacheKeyPolicy = CacheKeyProlicy.NoCache;
      CacheDuration = new TimeSpan( 0 );
    }

    /// <summary>
    /// 设置默认缓存键的产生依据
    /// </summary>
    public static CacheKeyProlicy CacheKeyPolicy
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
    /// 获取默认缓存键
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <returns>默认缓存键，根据缓存键依据设置而产生</returns>
    public static string GetCacheKey( HttpContextBase context )
    {
      if ( CacheKeyPolicy == Web.CacheKeyProlicy.NoCache )
        return null;

      var keys = new List<string>();

      if ( (CacheKeyPolicy & Web.CacheKeyProlicy.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          keys.Add( context.Session.SessionID );
      }

      if ( (CacheKeyPolicy & Web.CacheKeyProlicy.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          keys.Add( context.User.Identity.Name.Replace( ":", "::" ) );
      }

      if ( (CacheKeyPolicy & Web.CacheKeyProlicy.ByUrl) != 0 )
        keys.Add( context.Request.Url.AbsoluteUri );

      return string.Join( ":", keys.ToArray() );
    }

    /// <summary>
    /// 获取默认缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="handler">请求处理程序</param>
    /// <param name="cacheItem">缓存项</param>
    /// <returns>默认缓存策略</returns>
    public static HtmlCachePolicy GetPolicy( HttpContextBase context, IHtmlHandler handler, RawResponse cacheItem )
    {
      return new HtmlCachePolicy() { Duration = CacheDuration };
    }
  }


}
