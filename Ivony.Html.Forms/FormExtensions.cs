using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 提供一组关于表单的扩展方法
  /// </summary>
  public static class FormExtensions
  {

    /// <summary>
    /// 尝试将一个HTML元素转换为表单
    /// </summary>
    /// <param name="element">要转换为表单的元素</param>
    /// <param name="configuration">表单配置</param>
    /// <param name="provider">表单控件提供程序</param>
    /// <returns></returns>
    public static HtmlForm AsForm( this IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );


      return new HtmlForm( element, configuration, provider );
    }



  }
}
