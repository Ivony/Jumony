using System;
using System.Collections.Generic;
using System.Linq;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 提供元素导航查询的扩展方法
  /// </summary>
  public static class NavigateExtensions
  {


    private static void EnsureAvaliable( IHtmlNode node )
    {
      if ( node.Container == null )
        throw new InvalidOperationException( "无法对不存在于 DOM 上的节点进行操作" );
    }



    /// <summary>
    /// 获取父元素
    /// </summary>
    /// <param name="node">要获取父元素的节点</param>
    /// <returns>父元素</returns>
    public static IHtmlElement Parent( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );


      EnsureAvaliable( node );

      return node.Container as IHtmlElement;
    }




    /// <summary>
    /// 获取所有子元素
    /// </summary>
    /// <param name="container">要获取子元素的容器</param>
    /// <returns>容器的所有子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( this IHtmlContainer container )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      var nodes = container.Nodes();
      var nodeCollection = nodes as IHtmlNodeCollection;

      if ( nodeCollection != null )
        return nodeCollection.Elements();

      else
        return nodes.OfType<IHtmlElement>();
    }


    /// <summary>
    /// 获取符合条件的子元素
    /// </summary>
    /// <param name="container">要获取子元素的容器</param>
    /// <param name="selector">用来筛选子元素的元素选择器</param>
    /// <returns>符合条件的子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( this IHtmlContainer container, string selector )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( selector == null )
        throw new ArgumentNullException( "selector" );


      return CssParser.ParseElementSelector( selector ).Filter( Elements( container ) );
    }







    /// <summary>
    /// 获取所有父代元素
    /// </summary>
    /// <param name="node">要获取父代元素集合的节点</param>
    /// <returns>节点的所有父代元素集合</returns>
    public static IEnumerable<IHtmlElement> Ancestors( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      EnsureAvaliable( node );

      while ( true )
      {
        var element = node.Parent();


        if ( element == null )
          yield break;

        yield return element;


        node = element;

      }
    }

    /// <summary>
    /// 获取符合条件的父代元素
    /// </summary>
    /// <param name="node">要获取父代元素集合的节点</param>
    /// <param name="selector">用来筛选父代元素的元素选择器</param>
    /// <returns>节点的所有父代元素集合</returns>
    public static IEnumerable<IHtmlElement> Ancestors( this IHtmlNode node, string selector )
    {
      return CssParser.ParseElementSelector( selector ).Filter( Ancestors( node ) );
    }

    /// <summary>
    /// 获取所有的父代元素以及元素自身
    /// </summary>
    /// <param name="element">要获取父代元素及自身的元素</param>
    /// <returns>元素的所有父代元素和自身的集合</returns>
    public static IEnumerable<IHtmlElement> AncestorsAndSelf( this IHtmlElement element )
    {

      EnsureAvaliable( element );

      while ( true )
      {
        if ( element == null )
          yield break;

        yield return element;

        element = element.Parent();
      }
    }



    /// <summary>
    /// 获取所有的子代元素
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <returns>容器所有的子代元素</returns>
    public static IEnumerable<IHtmlElement> Descendants( this IHtmlContainer container )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );


      var nodes = container.Nodes();
      var nodeCollection = nodes as IHtmlNodeCollection;
      if ( nodeCollection != null )
        return nodeCollection.DescendantElements();

      else
        return DescendantNodesInternal( nodes ).OfType<IHtmlElement>();


    }


    /// <summary>
    /// 获取符合条件的子代元素
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <param name="selector">用于筛选子代元素的选择器</param>
    /// <returns>符合选择器的容器的所有子代元素</returns>
    /// <remarks>与Find方法不同的是，Descendants方法的选择器会无限上溯，即当判断父代约束时，会无限上溯到文档根。而Find方法只会上溯到自身的子节点</remarks>
    public static IEnumerable<IHtmlElement> Descendants( this IHtmlContainer container, string selector )
    {
      return CssParser.ParseSelector( selector ).Filter( Descendants( container ) );
    }



    /// <summary>
    /// 获取所有的子代节点
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <returns>容器所有的子代节点</returns>
    public static IEnumerable<IHtmlNode> DescendantNodes( this IHtmlContainer container )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );


      var nodes = container.Nodes();
      var nodeCollection = nodes as IHtmlNodeCollection;

      if ( nodeCollection != null )
        return nodeCollection.DescendantNodes();

      else
        return DescendantNodesInternal( nodes );
    }


    private static IEnumerable<IHtmlNode> DescendantNodesInternal( IEnumerable<IHtmlNode> nodes )
    {

      foreach ( var node in nodes )
      {
        yield return node;

        var childContainer = node as IHtmlContainer;
        if ( childContainer != null )
        {
          foreach ( var descendantNode in DescendantNodesInternal( childContainer.Nodes() ) )
            yield return descendantNode;
        }
      }
    }



    /// <summary>
    /// 获取所有的兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取兄弟节点的节点</param>
    /// <returns>所有的兄弟节点</returns>
    /// <exception cref="System.InvalidOperationException">如果节点不属于任何 HTML 容器</exception>
    public static IEnumerable<IHtmlNode> SiblingNodes( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      EnsureAvaliable( node );

      return node.Container.Nodes();

    }

    /// <summary>
    /// 获取所有的兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取兄弟（同级）元素节点的节点</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> Siblings( this IHtmlNode node )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      return node.SiblingNodes().OfType<IHtmlElement>();
    }

    /// <summary>
    /// 获取符合条件的兄弟（同级）元素节点
    /// </summary>
    /// <param name="node">要获取兄弟（同级）元素节点的节点</param>
    /// <param name="selector">用于筛选元素的元素选择器</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> Siblings( this IHtmlNode node, string selector )
    {
      return CssParser.ParseElementSelector( selector ).Filter( node.Siblings() );
    }


    /// <summary>
    /// 获取在自身之前的所有兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取之前的兄弟（同级）节点的节点</param>
    /// <returns>在这之后的所有兄弟（同级）节点</returns>
    public static IEnumerable<IHtmlNode> SiblingNodesBeforeSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().TakeWhile( n => !n.Equals( node ) );
    }

    /// <summary>
    /// 获取在之后的所有兄弟（同级）节点
    /// </summary>
    /// <param name="node">要获取之后的兄弟（同级）节点的节点</param>
    /// <returns>之后的所有兄弟（同级）节点</returns>
    public static IEnumerable<IHtmlNode> SiblingNodesAfterSelf( this IHtmlNode node )
    {
      return node.SiblingNodes().SkipWhile( n => !n.Equals( node ) ).Skip( 1 );
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
    /// 从当前容器按照 CSS 选择器搜索符合要求的元素
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS 选择器</param>
    /// <returns>搜索到的符合要求的元素</returns>
    public static IEnumerable<IHtmlElement> Find( this IHtmlContainer container, string expression )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      try
      {
        var selector = CssParser.Create( container, expression );
        return selector.Filter( container.Descendants() );
      }
      catch ( Exception e )
      {
        if ( e.Data != null && !e.Data.Contains( "selector expression" ) )
          e.Data["selector expression"] = expression;

        throw;
      }
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
    public static int ElementsIndexOfSelf( this IHtmlElement element )
    {
      var siblings = element.Siblings();
      return siblings.ToList().IndexOf( element );
    }


    /// <summary>
    /// 在元素集所有子代元素中使用 CSS 选择器选出符合要求的元素
    /// </summary>
    /// <param name="elements">作为选择范围的元素集</param>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的所有子代</returns>
    public static IEnumerable<IHtmlElement> Find( this IEnumerable<IHtmlElement> elements, string expression )
    {
      if ( !elements.Any() )
        return Enumerable.Empty<IHtmlElement>();

      if ( elements.IsSingle() )
        return elements.Single().Find( expression );

      var document = elements.First().Document;

      if ( elements.Any( e => !e.Document.Equals( document ) ) )
        throw new InvalidOperationException( "不支持在不同的文档中搜索" );

      var selector = CssCasecadingSelector.Create( elements, expression );

      return selector.Filter( document.Descendants() );

    }



    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的唯一元素，如果有多个元素符合要求，则会引发异常。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>搜索到的符合要求的唯一元素</returns>
    public static IHtmlElement FindSingle( this IHtmlContainer container, string expression )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      IHtmlElement result;

      try
      {
        result = container.Find( expression ).SingleOrDefault();
      }
      catch ( InvalidOperationException e )
      {
        throw new InvalidOperationException( string.Format( "符合选择器 \"{0}\" 的元素不唯一", expression ), e );
      }

      if ( result == null )
        throw new InvalidOperationException( string.Format( "未找到符合选择器 \"{0}\" 的元素。", expression ) );

      return result;
    }


    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的第一个元素，若不存在任何符合要求的元素，则抛出异常。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>搜索到的符合要求的第一个元素</returns>
    public static IHtmlElement FindFirst( this IHtmlContainer container, string expression )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );



      IHtmlElement result;

      result = container.Find( expression ).FirstOrDefault();

      if ( result == null )
        throw new InvalidOperationException( string.Format( "未找到符合选择器 \"{0}\" 的元素。", expression ) );

      return result;
    }


    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的最后一个元素，若不存在任何符合要求的元素，则抛出异常。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>搜索到的符合要求的最后一个元素</returns>
    public static IHtmlElement FindLast( this IHtmlContainer container, string expression )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );



      IHtmlElement result;

      result = container.Find( expression ).LastOrDefault();

      if ( result == null )
        throw new InvalidOperationException( string.Format( "未找到符合选择器 \"{0}\" 的元素。", expression ) );

      return result;
    }


    /// <summary>
    /// 确认在当前容器存在 CSS 选择器搜索符合要求的元素，若找到，则返回 true 。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <returns>是否存在符合要求的元素</returns>
    public static bool Exists( this IHtmlContainer container, string expression )
    {
      return container.Find( expression ).Any();
    }




    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的元素
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS 选择器</param>
    /// <param name="action">要对元素执行的操作</param>
    /// <returns>搜索到的符合要求的元素</returns>
    public static IEnumerable<IHtmlElement> Find( this IHtmlContainer container, string expression, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Find( expression ).ForAll( action );
    }


    /// <summary>
    /// 获取所有子节点，并做处理
    /// </summary>
    /// <param name="container">要获取子节点的容器</param>
    /// <param name="action">要对子节点进行的操作</param>
    /// <returns>容器的子节点</returns>
    public static IEnumerable<IHtmlNode> Nodes( IHtmlContainer container, Action<IHtmlNode> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Nodes().ForAll( action );
    }


    /// <summary>
    /// 获取所有元素，并做处理
    /// </summary>
    /// <param name="container">要获取子元素的容器</param>
    /// <param name="action">要对子元素进行的操作</param>
    /// <returns>容器的子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( IHtmlContainer container, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Elements().ForAll( action );
    }

    /// <summary>
    /// 获取所有子元素
    /// </summary>
    /// <param name="container">要获取子元素的容器</param>
    /// <param name="selector">用来筛选子元素的元素选择器</param>
    /// <param name="action">要对子元素执行的操作</param>
    /// <returns>容器的所有子元素</returns>
    public static IEnumerable<IHtmlElement> Elements( this IHtmlContainer container, string selector, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Elements( selector ).ForAll( action );
    }


    /// <summary>
    /// 获取所有的子代元素，并作相应处理
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <param name="action">要对子代元素执行的操作</param>
    /// <returns>容器所有的子代元素</returns>
    public static IEnumerable<IHtmlElement> Descendants( IHtmlContainer container, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Descendants().ForAll( action );
    }


    /// <summary>
    /// 获取符合条件的子代元素，并作相应处理
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <param name="selector">用于筛选子代元素的选择器</param>
    /// <param name="action">要对子代元素执行的操作</param>
    /// <returns>符合选择器的容器的所有子代元素</returns>
    /// <remarks>与Find方法不同的是，Descendants方法的选择器会无限上溯，即当判断父代约束时，会无限上溯到文档根。而Find方法只会上溯到自身的子节点</remarks>
    public static IEnumerable<IHtmlElement> Descendants( IHtmlContainer container, string selector, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.Descendants( selector ).ForAll( action );
    }


    /// <summary>
    /// 获取所有的子代节点，并作相应处理
    /// </summary>
    /// <param name="container">要获取子代元素的容器对象</param>
    /// <param name="action">要对子代元素执行的操作</param>
    /// <returns>容器所有的子代节点</returns>
    public static IEnumerable<IHtmlNode> DescendantNodes( IHtmlContainer container, Action<IHtmlNode> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return container.DescendantNodes().ForAll( action );
    }


    /// <summary>
    /// 获取所有的兄弟（同级）元素，并作相应处理
    /// </summary>
    /// <param name="node">要获取兄弟元素的节点</param>
    /// <param name="action">要对兄弟元素执行的操作</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    /// <exception cref="System.InvalidOperationException">如果节点不属于任何 HTML 容器</exception>
    public static IEnumerable<IHtmlElement> Siblings( IHtmlNode node, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return node.Siblings().ForAll( action );
    }


    /// <summary>
    /// 获取符合条件的兄弟（同级）元素节点，并作相应处理
    /// </summary>
    /// <param name="node">要获取兄弟（同级）元素节点的节点</param>
    /// <param name="selector">用于筛选元素的元素选择器</param>
    /// <param name="action">要对兄弟元素执行的操作</param>
    /// <returns>所有的兄弟（同级）元素节点</returns>
    public static IEnumerable<IHtmlElement> Siblings( IHtmlNode node, string selector, Action<IHtmlElement> action )
    {
      if ( action == null )
        throw new ArgumentNullException( "action" );

      return node.Siblings( selector ).ForAll( action );
    }


  }
}
