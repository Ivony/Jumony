using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 代表绑定表达式类型的枚举
  /// </summary>
  [Flags]
  public enum ExpressionType
  {

    /// <summary>默认值，使用这个值将引发错误</summary>
    None = 0,

    /// <summary>用元素表达的绑定表达式</summary>
    ElementExpression = 1,

    /// <summary>用属性表达的绑定表达式</summary>
    AttributeExpression = 2,

    /// <summary>用元素或属性表达的绑定表达式</summary>
    Both = 3

  }
}
