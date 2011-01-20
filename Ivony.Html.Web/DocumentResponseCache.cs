using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{
  public class DocumentResponse : RawResponse
  {
    private readonly IHtmlDocument cachedDocument;
    private string rendered;

    public DocumentResponse( IHtmlDocument document )
    {
      cachedDocument = document;
    }

    public IHtmlDocument Document
    {
      get { return cachedDocument; }
    }

    public string CachedResponse
    {
      get
      {
        if ( rendered == null )
          rendered = Document.Render();

        return rendered;
      }
    }

    public override void Output( HttpResponseBase response )
    {

      response.Write( CachedResponse );

    }
  }
}
