using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.ExpandedAPI
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


  }
}
