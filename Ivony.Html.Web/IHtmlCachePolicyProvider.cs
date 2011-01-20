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
}
