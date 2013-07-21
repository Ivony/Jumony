using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Ivony.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 实现 IViewLocationCache 接口
  /// </summary>
  public class JumonyViewLocationCache : IViewLocationCache
  {
    /// <summary>
    /// 获取视图位置
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="key">要查找的视图键</param>
    /// <returns>缓存的视图位置，如果有的话</returns>
    public string GetViewLocation( HttpContextBase httpContext, string key )
    {
      return (string) HttpRuntime.Cache.Get( key );
    }

    /// <summary>
    /// 插入视图位置缓存
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="key"></param>
    /// <param name="virtualPath"></param>
    public void InsertViewLocation( HttpContextBase httpContext, string key, string virtualPath )
    {
      HttpRuntime.Cache.Insert( key, virtualPath, null, Cache.NoAbsoluteExpiration, new TimeSpan( 0, 10, 0 ) );
    }
  }
}
