using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.ExpandedNavigateAPI
{
  public static class ExpandedNavigateExtensions
  {

    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的唯一元素，如果有多个元素符合要求，则会引发异常，如果没有符合要求的元素，则返回 defaultElement 或者 null 。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <param name="defaultElement">（可选）找不到符合要求的元素时应返回的元素，若不提供则返回 null</param>
    /// <returns>搜索到的符合要求的唯一元素</returns>
    public static IHtmlElement FindSingleOrDefault( this IHtmlContainer container, string expression, IHtmlElement defaultElement = null )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      try
      {
        return container.Find( expression ).SingleOrDefault() ?? defaultElement;
      }
      catch ( InvalidOperationException e )
      {
        throw new InvalidOperationException( string.Format( "符合选择器 \"{0}\" 的元素不唯一", expression ), e );
      }
    }


    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的第一个元素，若不存在任何符合要求的元素，则返回 defaultElement 或者 null 。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <param name="defaultElement">（可选）找不到符合要求的元素时应返回的元素，若不提供则返回 null</param>
    /// <returns>搜索到的符合要求的第一个元素</returns>
    public static IHtmlElement FindFirstOrDefault( this IHtmlContainer container, string expression, IHtmlElement defaultElement = null )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      return container.Find( expression ).FirstOrDefault() ?? defaultElement;
    }


    /// <summary>
    /// 从当前容器按照 CSS 选择器搜索符合要求的最后一个元素，若不存在任何符合要求的元素，则返回 defaultElement 或者 null 。
    /// </summary>
    /// <param name="container">要搜索子代元素的容器</param>
    /// <param name="expression">CSS选择器</param>
    /// <param name="defaultElement">（可选）找不到符合要求的元素时应返回的元素，若不提供则返回 null</param>
    /// <returns>搜索到的符合要求的最后一个元素</returns>
    public static IHtmlElement FindLastOrDefault( this IHtmlContainer container, string expression, IHtmlElement defaultElement = null )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );


      return container.Find( expression ).FirstOrDefault() ?? defaultElement;
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
