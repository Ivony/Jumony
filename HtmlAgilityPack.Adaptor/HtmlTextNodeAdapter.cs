using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public class HtmlTextNodeAdapter : HtmlDomObjectAdapter, IHtmlTextNode
  {
    public string HtmlText
    {
      get { return Node.InnerText; }
    }

    public HtmlTextNodeAdapter( AP.HtmlTextNode node )
      : base( node )
    {
    }


    #region IHtmlNode 成员

    public IHtmlContainer Container
    {
      get { return Node.ParentNode.AsContainer(); }
    }

    #endregion


  }
}
