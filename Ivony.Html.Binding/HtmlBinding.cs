using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{



  /// <summary>
  /// 为 HTML 绑定工作提供默认的元素绑定器，以及辅助创建数据绑定上下文和进行数据绑定
  /// </summary>
  public static class HtmlBinding
  {


    /// <summary>
    /// 获取样式绑定器
    /// </summary>
    public static StyleBinder StyleBinder { get; private set; }

    /// <summary>
    /// 获取文本绑定器
    /// </summary>
    public static LiteralBinder LiteralBinder { get; private set; }

    /// <summary>
    /// 获取表单绑定器
    /// </summary>
    public static FormBinder FormBinder { get; private set; }

    /// <summary>
    /// 获取脚本绑定器
    /// </summary>
    public static ScriptBinder ScriptBinder { get; private set; }

    /// <summary>
    /// 获取默认的绑定表达式绑定器
    /// </summary>
    public static EvalExpressionBinder EvalExpressionBinder { get; private set; }

    /// <summary>
    /// 获取默认的列表绑定表达式绑定器
    /// </summary>
    public static EvalListExpressionBinder EvalListExpressionBinder { get; private set; }



    static HtmlBinding()
    {
      StyleBinder = new StyleBinder();
      ScriptBinder = new ScriptBinder();
      LiteralBinder = new LiteralBinder();


      FormBinder = new FormBinder();


      EvalExpressionBinder = new EvalExpressionBinder();
      EvalListExpressionBinder = new EvalListExpressionBinder();



      ExpressionBinders = new ExpressionBinderCollection();
      ElementBinders = new List<IHtmlElementBinder>();

      ElementBinders.Add( StyleBinder );
      ElementBinders.Add( ScriptBinder );
      ElementBinders.Add( LiteralBinder );

      ExpressionBinders.Add( EvalExpressionBinder );
      ExpressionBinders.Add( EvalListExpressionBinder );
    }



    /// <summary>
    /// 获取或注册表达式绑定器
    /// </summary>
    public static ICollection<IExpressionBinder> ExpressionBinders
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取或设置所有元素绑定器
    /// </summary>
    public static ICollection<IHtmlElementBinder> ElementBinders
    {
      get;
      private set;
    }


    /// <summary>
    /// 使用默认的绑定器设置创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static HtmlBindingContext Create( IHtmlContainer scope, object dataContext )
    {
      return HtmlBindingContext.Create( ElementBinders.ToArray(), ExpressionBinders.ToArray(), scope, dataContext );
    }



    /// <summary>
    /// 使用默认的绑定器设置进行数据绑定
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static void DataBind( this IHtmlContainer scope, object dataContext )
    {
      Create( scope, dataContext ).DataBind();
    }
  }
}
