using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public abstract class HtmlHandlerAdapter : HtmlHandler
  {

    protected override void ProcessDocument()
    {

      Process();

    }


    protected override IHtmlDocument ParseDocument( string documentContent )
    {
      RawDocument = new HtmlDocument();

      RawDocument.LoadHtml( documentContent );

      return RawDocument.AsDocument();
    }

    protected HtmlDocument RawDocument
    {
      get;
      private set;
    }

    protected abstract void Process();

  }
}
