using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public class HtmlCachePolicy
  {

    public CacheDependency Dependency { get; set; }

    public TimeSpan Duration { get; set; }


  }
}
