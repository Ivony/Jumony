using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{


  /// <summary>
  /// 提供一个注册点，可以注册全局绑定表达式绑定器
  /// </summary>
  public static class GlobalExpressionBinders
  {


    static GlobalExpressionBinders()
    {
      _binders = new ExpressionBinderCollection();
      _binders.Add( new EvalExpressionBinder() );
    }

    private static ExpressionBinderCollection _binders;



    /// <summary>
    /// 获取已注册的全局绑定表达式绑定器列表
    /// </summary>
    public static ICollection<IExpressionBinder> Binders
    {
      get { return _binders; }
    }

  }
}
