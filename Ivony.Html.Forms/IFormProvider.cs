using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单控件提供程序需要实现的接口
  /// </summary>
  public interface IFormProvider
  {

    /// <summary>
    /// 尝试创建表单控件
    /// </summary>
    /// <param name="form">要创建控件的表单</param>
    /// <returns></returns>
    IFormControl[] DiscoveryControls( HtmlForm form );

  }
}
