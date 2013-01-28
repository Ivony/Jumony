using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;
using Ivony.Html.Forms;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 有关 MVC 的一些表单方法扩展
  /// </summary>
  public static class MvcFormExtensions
  {

    /// <summary>
    /// 在表单上应用指定模型的值
    /// </summary>
    /// <param name="form">要应用的表单</param>
    /// <param name="dataModel">要应用的模型值</param>
    /// <returns>返回表单，便于链式调用</returns>
    public static HtmlForm ApplyValues( this HtmlForm form, object dataModel )
    {

      var data = dataModel.ToPropertiesMap();

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {

        if ( data.ContainsKey( key ) )
          form[key].TrySetValue( data[key] );

      }

      return form;

    }



    /// <summary>
    /// 在表单上应用指定值提供程序的值
    /// </summary>
    /// <param name="form">要应用的表单</param>
    /// <param name="valueProvider">提供值的 ValueProvider 实例</param>
    /// <returns>返回表单，便于链式调用</returns>
    public static HtmlForm ApplyValues( this HtmlForm form, IValueProvider valueProvider )
    {

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {
        if ( valueProvider.ContainsPrefix( key ) )
        {
          form[key].TrySetValue( valueProvider.GetValue( key ).AttemptedValue );
        }
      }

      return form;

    }

  }
}
