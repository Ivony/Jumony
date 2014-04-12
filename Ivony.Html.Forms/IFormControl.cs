using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 代表一个表单控件
  /// </summary>
  public interface IFormControl
  {



    /// <summary>
    /// 控件所属的表单
    /// </summary>
    HtmlForm Form { get; }


    /// <summary>
    /// 控件名
    /// </summary>
    string Name { get; }


    /// <summary>
    /// 获取或设置控件目前的值
    /// </summary>
    string Value { get; set; }


    /// <summary>
    /// 检查值是否可以设置到控件
    /// </summary>
    /// <param name="value">要设置的值</param>
    /// <returns>是否可以设置</returns>
    bool CanSetValue( string value );


  }
}
