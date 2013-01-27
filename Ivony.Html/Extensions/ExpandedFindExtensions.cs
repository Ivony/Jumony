using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.ExpandedNavigateAPI
{
  public static class ExpandedFindExtensions
  {


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
    public static bool FindAny( this IHtmlContainer container, string expression )
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


  }
}
