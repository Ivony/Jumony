using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public interface IHtmlCachePolicy
  {

    public string CacheKey( HttpContextBase context );

    public TimeSpan GetCacheTime( HttpContextBase context, IHtmlHandler handler, IHtmlDocument document );
    public CacheDependency GetCacheDependency( HttpContextBase context, IHtmlHandler handler, IHtmlDocument document );

  }
}
