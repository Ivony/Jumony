using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

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
    public override BindingExpressionArgumentCollection Arguments
    {
      get
      {
        var arguments = new BindingExpressionArgumentCollection();
        Element.Attributes().ForAll( attribute => AddArgument( attribute, arguments ) );
        arguments.SetCompleted();
        return arguments;
      }
    }

    private void AddArgument( IHtmlAttribute attribute, BindingExpressionArgumentCollection arguments )
    {
      var expression = BindingExpression.ParseExpression( attribute.Value() );
      if ( expression != null )
        arguments.Add( attribute.Name, expression );

      else
        arguments.Add( attribute.Name, attribute.Value() );
    }
  }
}
