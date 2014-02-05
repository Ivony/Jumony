using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义一个可以将 BindingExpression 对象转换为字符串值的对象
  /// </summary>
  public interface IBindingExpressionEvaluator
  {

    /// <summary>
    /// 从 BindingExpression 获取需要绑定的值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定值</returns>
    string GetValue( BindingExpression bindingExpression );

  }
}
