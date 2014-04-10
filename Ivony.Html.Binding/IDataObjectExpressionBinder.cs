using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  /// <summary>
  /// 定义可以解析为一个对象而非字符串的绑定表达式
  /// </summary>
  public interface IDataObjectExpressionBinder : IExpressionBinder
  {

    /// <summary>
    /// 解析 DataContext 属性表达式，获取数据上下文
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="arguments">绑定参数</param>
    /// <returns>数据上下文</returns>
    object GetDataObject( HtmlBindingContext context, IDictionary<string, string> arguments );

  }
}
