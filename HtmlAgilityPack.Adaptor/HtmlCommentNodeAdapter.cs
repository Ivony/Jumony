using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  public class HtmlCommentNodeAdapter : HtmlNodeAdapter, IHtmlCommentNode
  {
    #region IHtmlCommentNode 成员

    public string Comment
    {
      get { return Node.InnerText; }
    }

    #endregion

    public HtmlCommentNodeAdapter( AP.HtmlNode node )
      : base( node )
    {
    }
  }
}
