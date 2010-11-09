using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public class HtmlParser : IHtmlParser
  {
    #region IHtmlParser 成员

    public IHtmlDocument Parse( string html )
    {
      var rawDocument = new HtmlDocument();

      rawDocument.LoadHtml( html );

      return rawDocument.AsDocument();

    }

    public HtmlFragment ParseFragment( string html )
    {
      var rawDocument = new HtmlDocument();

      rawDocument.LoadHtml( html );

      var fragment = new HtmlFragment();
      var document = rawDocument.AsDocument();
      
      fragment.AddNodesCopy( document.Nodes(), document.GetNodeFactory() );

      return fragment;
    }

    #endregion
  }
}
