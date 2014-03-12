using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义一个可以将 BindingExpression 对象转换为值的转换器，一般是绑定上下文
  /// </summary>
  public interface IBindingExpressionEvaluator
  {

    /// <summary>
    /// 尝试从 BindingExpression 获取需要绑定的字符串值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>获取到的值，若不支持该绑定表达式或者无法解析，则返回null</returns>
    string GetValue( BindingExpression expression );

  }
}
