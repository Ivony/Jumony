using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 处理{eval-list xxx}绑定表达式的绑定器
  /// </summary>
  public class EvalListExpressionBinder : EvalExpressionBinder, IExpressionBinder, IDataObjectExpressionBinder
  {


    string IExpressionBinder.ExpressionName
    {
      get { return "eval-list"; }
    }

    string IExpressionBinder.GetValue( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      throw new NotImplementedException();
    }

    object IDataObjectExpressionBinder.GetDataObject( HtmlBindingContext context, IDictionary<string, string> arguments )
    {
      throw new NotImplementedException();
    }
  }
}
