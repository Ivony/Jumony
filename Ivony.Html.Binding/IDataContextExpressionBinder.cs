using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  /// <summary>
  /// 定义可用于解析 datacontext 属性绑定表达式的绑定器
  /// </summary>
  public interface IDataContextExpressionBinder : IExpressionBinder
  {

    /// <summary>
    /// 解析 DataContext 属性表达式，获取数据上下文
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>数据上下文</returns>
    object GetDataContext( HtmlBindingContext context, IDictionary<string, string> arguments );

  }
}
