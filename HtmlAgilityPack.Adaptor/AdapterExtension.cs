using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  public static class AdapterExtension
  {

    public static IEnumerable<HtmlNode> Find( this HtmlNode node, string expression )
    {
      var selector = HtmlCssSelector.Create( expression );
      return selector.Search( node.AsContainer(), true ).Select( element => (HtmlNode) element.NodeObject );
    }

    public static IEnumerable<HtmlNode> Find( this HtmlDocument document, string expression )
    {
      return document.DocumentNode.Find( expression );
    }

  }
}
