using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Web;

namespace Ivony.Html.Web
{
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

      if ( CacheKeyPolicy == CacheKeyPolicy.NoCache )
        return null;

      CacheToken token = null;

      if ( (CacheKeyPolicy & CacheKeyPolicy.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          token += CacheToken.FromSessionID( context );
      }

      if ( (CacheKeyPolicy & CacheKeyPolicy.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          token += CacheToken.FromCookies( context );
      }

      if ( (CacheKeyPolicy & CacheKeyPolicy.ByVirtualPath) != 0 )
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
