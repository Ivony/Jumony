using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Fluent;
using System.Web;

namespace Ivony.Html
{

  /// <summary>
  /// 提供元素导航查询的扩展方法
  /// </summary>
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
    /// 获取容器符合条件的子元素
    /// </summary>
    /// <param name="node">要获取子元素的容器</param>
    /// <param name="selector">用来筛选子元素的元素选择器</param>
    /// <returns>符合条件的子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( this IHtmlContainer node, string selector )
    {
      return HtmlCssSelector.CreateElementSelector( selector ).Search( Elements( node ) );
    }

    /// <summary>
    /// 获取节点的所有父代元素
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
    /// 获取节点符合条件的父代元素
    /// </summary>
    /// <param name="node">要获取父代元素集合的节点</param>
    /// <param name="selector">用来筛选父代元素的元素选择器</param>
    /// <returns>节点的所有父代元素集合</returns>
    public static IEnumerable<IHtmlElement> Ancestors( this IHtmlNode node, string selector )
    {
      return HtmlCssSelector.CreateElementSelector( selector ).Search( Ancestors( node ) );
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



    /// <summary>
    /// 获取所有的子代元素
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <returns>容器所有的子代元素</returns>
    public static IEnumerable<IHtmlElement> Descendants( this IHtmlContainer container )
    {
      return container.DescendantNodes().OfType<IHtmlElement>();
    }

    /// <summary>
    /// 获取所有的子代元素
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <param name="selector">用于筛选子代元素的选择器</param>
    /// <returns>符合选择器的容器的所有子代元素</returns>
    /// <remarks>与Find方法不同的是，Descendants方法的选择器会无限上溯，即当判断父代约束时，会无限上溯到文档根。而Find方法只会上溯到自身的子节点</remarks>
    public static IEnumerable<IHtmlElement> Descendants( this IHtmlContainer container, string selector )
    {
      return HtmlCssSelector.Create( selector ).Search( container, false );
    }

    /// <summary>
    /// 获取所有的子代节点
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <returns>容器所有的子代节点</returns>
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



    /// <summary>
    /// 获取所有的兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取兄弟节点的节点</param>
    /// <returns>所有的兄弟节点</returns>
    public static IEnumerable<IHtmlNode> SiblingNodes( this IHtmlNode node )
    {
      var parent = node.Parent;

      if ( parent == null )
        return new IHtmlNode[] { node };

      return parent.Nodes();
    }

    /// <summary>
    /// 获取所有的兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取兄弟（同级）元素节点的节点</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> Siblings( this IHtmlNode node )
    {
      return node.SiblingNodes().OfType<IHtmlElement>();
    }

    /// <summary>
    /// 获取所有的兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取兄弟（同级）元素节点的节点</param>
    /// <param name="selector">用于筛选元素的元素选择器</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> Siblings( this IHtmlNode node, string selector )
    {
      return HtmlCssSelector.CreateElementSelector( selector ).Search( node.Siblings() );
    }


    /// <summary>
    /// 获取在自身之前的所有兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取之前的兄弟（同级）节点的节点</param>
    /// <returns>在这之后的所有兄弟（同级）节点</returns>
    public static IEnumerable<IHtmlNode> SiblingNodesBeforeSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().TakeWhile( n => !n.NodeObject.Equals( node.NodeObject ) );
    }

    /// <summary>
    /// 获取在之后的所有兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取之后的兄弟（同级）节点的节点</param>
    /// <returns>之后的所有兄弟（同级）节点</returns>
    public static IEnumerable<IHtmlNode> SiblingNodesAfterSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().SkipWhile( n => !n.NodeObject.Equals( node.NodeObject ) ).Skip( 1 );
    }


    /// <summary>
    /// 获取在自身之前的所有兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取之前的兄弟（同级）元素节点的节点</param>
    /// <returns>在这之后的所有兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> SiblingsBeforeSelf( this IHtmlNode node )
    {
      return node.SiblingNodesBeforeSelf().OfType<IHtmlElement>();
    }

    /// <summary>
    /// 获取在之后的所有兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取之后的兄弟（同级）元素节点的节点</param>
    /// <returns>之后的所有兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> SiblingsAfterSelf( this IHtmlNode node )
    {
      return node.SiblingNodesAfterSelf().OfType<IHtmlElement>();
    }




    /// <summary>
    /// 获取紧邻之前的元素
    /// </summary>
    /// <param name="node">要获取紧邻之前的元素的节点</param>
    /// <returns>紧邻当前节点的前一个元素</returns>
    public static IHtmlElement PreviousElement( this IHtmlNode node )
    {
      return node.SiblingsBeforeSelf().LastOrDefault();
    }


    /// <summary>
    /// 获取紧邻之后的元素
    /// </summary>
    /// <param name="node">要获取紧邻之后的元素的节点</param>
    /// <returns>紧邻当前节点的后一个元素</returns>
    public static IHtmlElement NextElement( this IHtmlNode node )
    {
      return node.SiblingsAfterSelf().FirstOrDefault();
    }


    /// <summary>
    /// 获取紧邻之前的节点
    /// </summary>
    /// <param name="node">要获取紧邻之前的元素的节点</param>
    /// <returns>紧邻当前节点的前一个节点</returns>
    public static IHtmlNode PreviousNode( this IHtmlNode node )
    {
      return node.SiblingNodesBeforeSelf().LastOrDefault();
    }

    /// <summary>
    /// 获取紧邻之后的节点
    /// </summary>
    /// <param name="node">要获取紧邻之后的元素的节点</param>
    /// <returns>紧邻当前节点的后一个节点</returns>
    public static IHtmlNode NextNode( this IHtmlNode node )
    {
      return node.SiblingNodesAfterSelf().FirstOrDefault();
    }


    /// <summary>
    /// 从当前容器按照CSS3选择器搜索符合要求的元素
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>搜索到的符合要求的元素</returns>
    public static IEnumerable<IHtmlElement> Find( this IHtmlContainer container, string expression )
    {
      var selector = HtmlCssSelector.Create( expression );
      return selector.Search( container, true );
    }


    /// <summary>
    /// 从当前容器按照CSS3选择器搜索符合要求的唯一元素，如果有多个元素符合要求，则会引发异常。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>搜索到的符合要求的唯一元素</returns>
    public static IHtmlElement FindSingle( this IHtmlContainer container, string expression )
    {
      return Find( container, expression ).Single();
    }


    /// <summary>
    /// 从当前容器按照CSS3选择器搜索符合要求的元素
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expressions">多个CSS选择器，结果会合并</param>
    /// <returns>搜索到的符合要求的元素</returns>
    public static IEnumerable<IHtmlElement> Find( this IHtmlContainer container, params string[] expressions )
    {
      var selector = HtmlCssSelector.Create( expressions );
      return selector.Search( container, true );
    }


    /// <summary>
    /// 获取在兄弟节点中，自己的顺序位置
    /// </summary>
    /// <param name="node">要获取序号的节点</param>
    /// <returns>顺序位置</returns>
    public static int NodesIndexOfSelf( this IHtmlNode node )
    {
      var siblings = node.SiblingNodes();
      return siblings.ToList().IndexOf( node );
    }


    /// <summary>
    /// 获取在兄弟元素中，自己的顺序位置
    /// </summary>
    /// <param name="element">要获取序号的元素</param>
    /// <returns>顺序位置</returns>
    public static int IndexOfSelf( this IHtmlElement element )
    {
      var siblings = element.Siblings();
      return siblings.ToList().IndexOf( element );
    }

  }
}
