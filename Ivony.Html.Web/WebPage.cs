using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class WebPage : HtmlDocumentWrapper
  {

    public WebPage( IHtmlDocument document, Uri url )
    {
      _document = document;
      _url = url;

    }

    private Uri _url;

    private IHtmlDocument _document;

    protected override IHtmlDocument Document
    {
      get { return _document; }
    }

    public Uri Url
    {
      get { return _url; }
    }



    

  }
}
