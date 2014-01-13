using Ivony.Html.Web.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{


  public class DefaultBinderProvider : IHtmlBinderProvider
  {


    public static StyleBinder StyleBinder { get; private set; }
    public static FormBinder FormBinder { get; private set; }
    public static ScriptBinder ScriptBinder { get; private set; }
    public static BindingExpressionBinder BindingExpressionBinder { get; private set; }

    public IHtmlBinder[] GetHtmlBinders()
    {
      return new IHtmlBinder[] { StyleBinder, FormBinder, ScriptBinder };
    }

    public IExpressionBinder[] GetExpressionBinders()
    {
      return new IExpressionBinder[] { BindingExpressionBinder };
    }
  }
}
