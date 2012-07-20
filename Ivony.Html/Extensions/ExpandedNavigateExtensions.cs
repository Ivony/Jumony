using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.ExpandedAPI
{
  public static class ExpandedNavigateExtensions
  {

    public static IEnumerable<IHtmlNode> Nodes( IHtmlContainer container, Action<IHtmlNode> action )
    {
      return container.Nodes().ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Elements( IHtmlContainer container, Action<IHtmlElement> action )
    {
      return container.Elements().ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Elements( IHtmlContainer container, string selector, Action<IHtmlElement> action )
    {
      return container.Elements( selector ).ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Descendants( IHtmlContainer container, Action<IHtmlElement> action )
    {
      return container.Descendants().ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Descendants( IHtmlContainer container, string selector, Action<IHtmlElement> action )
    {
      return container.Descendants( selector ).ForAll( action );
    }

    public static IEnumerable<IHtmlNode> DescendantNodes( IHtmlContainer container, Action<IHtmlNode> action )
    {
      return container.DescendantNodes().ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Siblings( IHtmlNode node, Action<IHtmlElement> action )
    {
      return node.Siblings().ForAll( action );
    }

    public static IEnumerable<IHtmlElement> Siblings( IHtmlNode node, string selector, Action<IHtmlElement> action )
    {
      return node.Siblings( selector ).ForAll( action );
    }

  }
}
