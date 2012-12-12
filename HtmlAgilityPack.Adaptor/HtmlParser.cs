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

    public IHtmlDocument Parse( string html, Uri url )
    {
      if ( html == null )
        throw new ArgumentNullException("html");

      if ( url != null && !url.IsAbsoluteUri )
        throw new ArgumentException( "必须是绝对URI", "url" );


      var rawDocument = new HtmlDocument();

      if ( !string.IsNullOrEmpty( html ) )
        rawDocument.LoadHtml( html );

      if ( url != null )
        rawDocument.DocumentNode.SetAttributeValue( "uri", url.AbsoluteUri );

      return new HtmlDocumentAdapter( rawDocument );

    }

    #endregion
  }
}
