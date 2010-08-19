using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Fluent;

namespace Ivony.Web.Html
{
  public static class ElementExtensions
  {

    /// <summary>
    /// 获取容器所有子元素
    /// </summary>
    /// <param name="node">要获取子元素的容器</param>
    /// <returns>容器的所有子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( this IHtmlContainer node )
    {
      return node.Nodes().OfType<IHtmlElement>();
    }

    /// <summary>
    /// 获取节点的所有父代元素集合
    /// </summary>
    /// <param name="node">要获取父代元素集合的节点</param>
    /// <returns>节点的所有父代元素集合</returns>
    public static IEnumerable<IHtmlElement> Ancestors( this IHtmlNode node )
    {
      while ( true )
      {
        node = node.Parent;

        var element = node as IHtmlElement;

        if ( element == null )
          yield break;

        yield return element;

      }
    }

    /// <summary>
    /// 获取元素所有的父代元素以及元素自身
    /// </summary>
    /// <param name="element">要获取父代元素及自身的元素</param>
    /// <returns>元素的所有父代元素和自身的集合</returns>
    public static IEnumerable<IHtmlElement> AncestorsAndSelf( this IHtmlElement element )
    {
      while ( true )
      {
        if ( element == null )
          yield break;

        yield return element;

        element = element.Parent as IHtmlElement;
      }
    }



    public static IEnumerable<IHtmlElement> Descendant( this IHtmlContainer container )
    {
      return container.DescendantNodes().OfType<IHtmlElement>();
    }

    public static IEnumerable<IHtmlNode> DescendantNodes( this IHtmlContainer container )
    {

      foreach ( var node in container.Nodes() )
      {
        yield return node;

        var childContainer = node as IHtmlContainer;
        if ( childContainer != null )
        {
          foreach ( var descendantNode in DescendantNodes( childContainer ) )
            yield return descendantNode;
        }
      }

    }



    public static IEnumerable<IHtmlNode> SiblingNodes( this IHtmlNode node )
    {
      var parent = node.Parent;

      if ( parent == null )
        return new IHtmlNode[] { node };

      return parent.Nodes();
    }

    public static IEnumerable<IHtmlElement> SiblingElements( this IHtmlNode node )
    {
      return node.SiblingNodes().OfType<IHtmlElement>();
    }


    public static IEnumerable<IHtmlElement> ElementsBeforeSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().TakeWhile( n => !n.NodeObject.Equals( node.NodeObject ) ).OfType<IHtmlElement>();
    }

    public static IEnumerable<IHtmlElement> ElementsAfterSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().SkipWhile( n => !n.NodeObject.Equals( node.NodeObject ) ).OfType<IHtmlElement>();
    }


    public static IHtmlElement PreviousElement( this IHtmlNode node )
    {
      return node.ElementsBeforeSelf().LastOrDefault();
    }

    public static IHtmlElement NextElement( this IHtmlNode node )
    {
      return node.ElementsAfterSelf().FirstOrDefault();
    }



    public static IEnumerable<IHtmlElement> Find( this IHtmlContainer container, string expression )
    {
      var selector = HtmlCssSelector.Create( expression );
      return selector.Search( container, true );
    }





    public static int NodeInedx( this IHtmlNode node )
    {
      var siblings = node.SiblingNodes();
      return siblings.ToList().IndexOf( node );
    }

    public static int ElementIndex( this IHtmlElement element )
    {
      var siblings = element.SiblingElements();
      return siblings.ToList().IndexOf( element );
    }
  }
}
