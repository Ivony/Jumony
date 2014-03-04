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
    public static EvalExpressionBinder BindingExpressionBinder { get; private set; }


    static HtmlBinding()
    {
      StyleBinder = new StyleBinder();
      BindingExpressionBinder = new EvalExpressionBinder();

      FormBinder = new FormBinder();
      ScriptBinder = new ScriptBinder();
      LiteralBinder = new LiteralBinder();
    }


    /// <summary>
    /// 使用默认的绑定器设置创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static HtmlBindingContext Create( IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues )
    {
      return HtmlBindingContext.Create( new IHtmlBinder[] { StyleBinder, LiteralBinder, ScriptBinder }, new IExpressionBinder[] { BindingExpressionBinder }, scope, dataContext, dataValues );
    }



    /// <summary>
    /// 使用默认的绑定器设置进行数据绑定
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static void DataBind( IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues )
    {
      Create( scope, dataContext, dataValues ).DataBind();
    }
  }
}
