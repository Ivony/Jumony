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
  public class EvalListExpressionBinder : EvalExpressionBinder, IExpressionBinder
  {


    string IExpressionBinder.ExpressionName
    {
      get { return "eval-list"; }
    }


    object IExpressionBinder.GetValue( HtmlBindingContext context, BindingExpression expression )
    {

      var dataModel = GetDataObject( context, expression );

      if ( dataModel == null )
        return null;


      CssElementSelector elementSelector = null;
      string selector;

      if ( expression.TryGetValue( context, "selector", out selector ) )
        elementSelector = CssParser.ParseElementSelector( selector );

      ListBindingMode mode;

      string modeSetting;
      if ( expression.TryGetValue( context, "mode", out modeSetting ) && modeSetting.EqualsIgnoreCase( "static" ) )
        mode = ListBindingMode.StaticContent;
      
      else
        mode = ListBindingMode.DynamicContent;



      return new ListDataModel( (IEnumerable) dataModel, elementSelector, mode );

    }
  }
}
