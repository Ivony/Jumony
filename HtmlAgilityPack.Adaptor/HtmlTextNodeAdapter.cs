using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public class HtmlTextNodeAdapter : HtmlNodeAdapter, IHtmlTextNode
  {
    public string HtmlText
    {
      get { return Node.InnerText; }
    }

    public HtmlTextNodeAdapter( AP.HtmlNode node )
      : base( node )
    {
    }

  }
}
