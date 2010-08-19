using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlDocumentAdapter : HtmlContainerAdapter, IHtmlDocument
  {

    public HtmlDocumentAdapter( AP.HtmlDocument document )
      : base( document.DocumentNode )
    {
    }

    public string DocumentDeclaration
    {
      get { return null; }
    }
  }
}
