using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public static class AdapterExtension
  {

    public static IEnumerable<HtmlNode> Find( this HtmlNode node, params string[] expressions )
    {
      var selector = HtmlCssSelector.Create( expressions );
      return selector.Search( node.AsContainer(), true ).Select( element => (HtmlNode) element.RawObject );
    }

    public static IEnumerable<HtmlNode> Find( this HtmlDocument document, params string[] expressions )
    {
      return document.DocumentNode.Find( expressions );
    }

  }
}
