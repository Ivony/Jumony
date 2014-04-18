using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

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
      throw new NotSupportedException();
    }

    object IDataObjectExpressionBinder.GetDataObject( HtmlBindingContext context, IDictionary<string, string> arguments )
    {

      var dataModel = GetDataObject( context, arguments );

      if ( dataModel == null )
        return null;


      CssElementSelector elementSelector = null;
      string selector;

      if ( arguments.TryGetValue( "selector", out selector ) )
      {
        elementSelector = CssParser.ParseElementSelector( selector );
      }


      return new ListDataModel( (IEnumerable) dataModel, ListBindingMode.Repeat );

    }
  }
}
