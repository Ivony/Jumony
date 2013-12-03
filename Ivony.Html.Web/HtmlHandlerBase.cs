using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 处理 HTML 文档或文档部分的处理器基类
  /// </summary>
  public abstract class HtmlHandlerBase : HttpHandlerBase
  {


    /// <summary>
    /// 获取要处理的范畴
    /// </summary>
    protected abstract IHtmlContainer HtmlScope { get; }

    /// <summary>
    /// 在处理范畴内查找符合选择器的元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的元素</returns>
    protected IEnumerable<IHtmlElement> Find( string expression )
    {
      return HtmlScope.Find( expression );
    }


    /// <summary>
    /// 在处理范畴内查找符合选择器的唯一元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的唯一元素</returns>
    protected IHtmlElement FindSingle( string expression )
    {
      return HtmlScope.FindSingle( expression );
    }

    /// <summary>
    /// 在处理范畴内查找符合选择器的首个元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的首个元素</returns>
    protected IHtmlElement FindFirst( string expression )
    {
      return HtmlScope.FindFirst( expression );
    }

    /// <summary>
    /// 在处理范畴内查找符合选择器的最后一个元素
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <returns>符合选择器的最后一个元素</returns>
    protected IHtmlElement FindLast( string expression )
    {
      return HtmlScope.FindLast( expression );
    }



    /// <summary>
    /// 对处理范畴内查找符合选择器的首个元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForFirst( string expression, Action<IHtmlElement> action )
    {
      HtmlScope.Find( expression ).ForFirst( action );
    }

    /// <summary>
    /// 对处理范畴内查找符合选择器的唯一元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForSingle( string expression, Action<IHtmlElement> action )
    {
      HtmlScope.Find( expression ).ForSingle( action );
    }

    /// <summary>
    /// 对处理范畴内查找符合选择器的最后一个元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForLast( string expression, Action<IHtmlElement> action )
    {
      HtmlScope.Find( expression ).ForLast( action );
    }


    /// <summary>
    /// 对处理范畴内查找符合选择器的所有元素进行处理
    /// </summary>
    /// <param name="expression">CSS 选择器表达式</param>
    /// <param name="action">要进行的处理</param>
    protected void ForAll( string expression, Action<IHtmlElement> action )
    {
      HtmlScope.Find( expression ).ForAll( action );
    }

  }
}
