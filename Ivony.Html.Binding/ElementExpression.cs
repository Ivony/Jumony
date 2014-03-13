using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 代表用元素表示的绑定表达式
  /// </summary>
  internal sealed class ElementExpression : BindingExpression
  {


    public IHtmlElement Element
    {
      get;
      private set;
    }


    /// <summary>
    /// 根据元素创建一个元素绑定表达式
    /// </summary>
    /// <param name="element"></param>
    public ElementExpression( IHtmlElement element )
    {
      Element = element;
    }


    /// <summary>
    /// 绑定表达式名称
    /// </summary>
    public override string Name
    {
      get { return Element.Name; }
    }


    /// <summary>
    /// 绑定表达式参数
    /// </summary>
    public override IDictionary<string, string> Arguments
    {
      get { return Element.Attributes().ToDictionary( a => a.Name, a => a.AttributeValue, StringComparer.OrdinalIgnoreCase ); }
    }
  }
}
