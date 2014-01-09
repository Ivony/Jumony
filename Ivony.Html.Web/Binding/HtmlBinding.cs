using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
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
  }
}
