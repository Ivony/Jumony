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
    /// 尝试将字符串转换为指定类型的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="obj">要转换的源对象</param>
    /// <param name="value">转换后的值</param>
    /// <returns>是否转换成功</returns>
    bool TryConvertValue<T>( object obj, out T value );

    /// <summary>
    /// 从 BindingExpression 获取需要绑定的值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>获取到的绑定值</returns>
    object GetValue( BindingExpression expression );
  }
}
