using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{




  /// <summary>
  /// 定义绑定表达式，绑定表达式可以由属性值或者元素来定义。
  /// </summary>
  public abstract class BindingExpression
  {


    /// <summary>
    /// 用于解析属性表达式参数的正则表达式
    /// </summary>
    public static readonly string ExpressionArgumentPattern = @"(?<args>(?<name>\w+)(=(?<value>[^\,]+))?)";

    /// <summary>
    /// 用于解析属性表达式的正则表达式
    /// </summary>
    public static readonly string AttributeExpressionPattern = string.Format( @"(?<expression>\{{(?<ename>\w+)\s*({0}(\,\s*{0})*)?\}})", ExpressionArgumentPattern );


    /// <summary>
    /// 表达式名称
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 参数值
    /// </summary>
    public abstract IDictionary<string, string> Arguments { get; }

  }
}
