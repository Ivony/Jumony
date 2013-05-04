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
  public class JumonyViewLocationCache : IViewLocationCache
  {
    public string GetViewLocation( HttpContextBase httpContext, string key )
    {
      return (string) HttpRuntime.Cache.Get( key );
    }

    public void InsertViewLocation( HttpContextBase httpContext, string key, string virtualPath )
    {
      HttpRuntime.Cache.Insert( key, virtualPath, null, Cache.NoAbsoluteExpiration, new TimeSpan( 0, 10, 0 ) );
    }
  }
}
