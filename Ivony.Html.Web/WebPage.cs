using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class WebPage
  {

    public WebPage( IHtmlDocument document, Uri url, string cacheKey )
    {
      Document = document;
      Url = url;
      CacheKey = cacheKey;
    }


    public override IHtmlDocument Document
    {
      private set;
      get;
    }

    public Uri Url
    {
      private set;
      get;
    }


    public string CacheKey
    {
      private set;
      get;
    }



  }
}
