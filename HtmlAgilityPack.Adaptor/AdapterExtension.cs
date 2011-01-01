using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public static class AdapterExtension
  {

    public static IEnumerable<HtmlNode> Find( this HtmlNode node, string expression )
    {
      var container = node.AsContainer();
      return CssSelector.Search( expression, container ).Select( element => (HtmlNode) element.RawObject );
    }

    public static IEnumerable<HtmlNode> Find( this HtmlDocument document, string expression )
    {
      return document.DocumentNode.Find( expression );
    }

  }
}
