using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public interface IHtmlCachePolicyProvider
  {

    string GetCacheKey( HttpContextBase context );

    HtmlCachePolicy GetPolicy( HttpContextBase context, IHtmlHandler handler, RawResponse cacheItem );

  }


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


  public static class DefaultCachePolicies
  {


    static DefaultCachePolicies()
    {
      CacheKeyBasis = CacheKeyProlicy.NoCache;
      CacheDuration = new TimeSpan( 0 );
    }

    /// <summary>
    /// 设置默认缓存键的产生依据
    /// </summary>
    public static CacheKeyProlicy CacheKeyBasis
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


    public static string GetCacheKey( HttpContextBase context )
    {
      if ( CacheKeyBasis == Web.CacheKeyProlicy.NoCache )
        return null;

      var keys = new List<string>();

      if ( (CacheKeyBasis & Web.CacheKeyProlicy.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          keys.Add( context.Session.SessionID );
      }

      if ( (CacheKeyBasis & Web.CacheKeyProlicy.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          keys.Add( context.User.Identity.Name.Replace( ":", "::" ) );
      }

      if ( (CacheKeyBasis & Web.CacheKeyProlicy.ByUrl) != 0 )
        keys.Add( context.Request.Url.AbsoluteUri );

      return string.Join( ":", keys.ToArray() );
    }

    public static HtmlCachePolicy GetPolicy( HttpContextBase context, IHtmlHandler handler, RawResponse cacheItem )
    {
      return new HtmlCachePolicy() { Duration = CacheDuration };
    }
  }


}
