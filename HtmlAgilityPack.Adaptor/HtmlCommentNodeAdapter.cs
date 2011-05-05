using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public class HtmlCommentNodeAdapter : HtmlDomObjectAdapter, IHtmlComment, IHtmlSpecial
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


    #region IHtmlNode 成员

    public IHtmlContainer Container
    {
      get { return Node.ParentNode.AsContainer(); }
    }

    #endregion

  }
}
