using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Binding;

namespace Ivony.Html.Web
{


  public class HtmlBinding
  {


    public static StyleBinder StyleBinder { get; private set; }
    public static FormBinder FormBinder { get; private set; }
    public static ScriptBinder ScriptBinder { get; private set; }
    public static BindingExpressionBinder BindingExpressionBinder { get; private set; }


    static HtmlBinding()
    {
      StyleBinder = new StyleBinder();
      BindingExpressionBinder = new BindingExpressionBinder();

      FormBinder = new FormBinder();
      ScriptBinder = new ScriptBinder();
    }


    internal static IHtmlBinder[] GetHtmlBinders( HtmlRequestContext requestContext )
    {
      return new IHtmlBinder[] { StyleBinder };
    }

    internal static ExpressionBinderCollection GetExpressionBinders( HtmlRequestContext requestContext )
    {
      return new ExpressionBinderCollection(){ BindingExpressionBinder };
    }

    /// <summary>
    /// 使用默认的绑定器设置创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static HtmlBindingContext Create( IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues )
    {
      return HtmlBindingContext.Create( new IHtmlBinder[] { HtmlBinding.StyleBinder }, new IExpressionBinder[] { HtmlBinding.BindingExpressionBinder }, scope, dataContext, dataValues );
    }


  }
}
