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


  public static class DefaultCachePolicies
  {

    /// <summary>
    /// 设置默认缓存键的产生依据
    /// </summary>
    public static CacheKeyBasis CacheKeyBasis
    {
      get;
      set;
    }


    /// <summary>
    /// 获取或设置默认缓存时间
    /// </summary>
    public static TimeSpan Duration
    {
      get;
      set;
    }


    public static string GetCacheKey( HttpContextBase context )
    {
      if ( CacheKeyBasis == Web.CacheKeyBasis.NoCache )
        return null;

      var keys = new List<string>();

      if ( (CacheKeyBasis & Web.CacheKeyBasis.BySession) != 0 )
      {
        if ( !context.Session.IsCookieless && !context.Session.IsNewSession )
          keys.Add( context.Session.SessionID );
      }

      if ( (CacheKeyBasis & Web.CacheKeyBasis.ByIdentity) != 0 )
      {
        if ( context.User != null && context.User.Identity != null && context.User.Identity.Name != null )
          keys.Add( context.User.Identity.Name.Replace( ":", "::" ) );
      }

      if ( (CacheKeyBasis & Web.CacheKeyBasis.ByUrl) != 0 )
        keys.Add( context.Request.Url.AbsoluteUri );

      return string.Join( ":", keys.ToArray() );
    }

    public static HtmlCachePolicy GetPolicy( HttpContextBase context, IHtmlHandler handler, RawResponse cacheItem )
    {
      return new HtmlCachePolicy() { Duration = Duration };
    }
  }


}
