using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{
  public class WebPageResponse : RawResponse
  {

    private WebPage cachedPage;

    public WebPageResponse( WebPage page )
    {
      cachedPage = page;
      Content = page.Render();
    }

    public WebPage Page
    {
      get { return cachedPage; }
    }

  }
}
