using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单控件验证器
  /// </summary>
  public interface IFormFieldValidator
  {

    /// <summary>
    /// 字段名
    /// </summary>
    string Name { get; }


    /// <summary>
    /// 对控件值进行验证
    /// </summary>
    /// <param name="value">控件提交的值</param>
    /// <returns>验证错误对象，若没有错误则返回 null</returns>
    FormValidationError Validate( string value );

  }
}
