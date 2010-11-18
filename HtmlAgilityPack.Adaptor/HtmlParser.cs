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

      if ( !string.IsNullOrEmpty( html ) )
        rawDocument.LoadHtml( html );

      return rawDocument.AsDocument();

    }

    #endregion
  }
}
