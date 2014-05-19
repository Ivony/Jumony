using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义处理绑定表达式的绑定器
  /// </summary>
  public interface IExpressionBinder
  {

    /// <summary>
    /// 要处理的绑定表达式的名称
    /// </summary>
    string ExpressionName { get; }



    /// <summary>
    /// 进行绑定，根据给定的参数获取需要绑定的值
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定的值</returns>
    object GetValue( HtmlBindingContext context, BindingExpression expression );


  }
}
