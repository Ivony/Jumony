using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 实现 IFormValidator
  /// </summary>
  public class FormValidator : IFormValidator
  {


    /// <summary>
    /// 创建 FormValidator 对象
    /// </summary>
    /// <param name="validators">字段验证器列表</param>
    public FormValidator( FormFieldValidatorCollection validators )
    {
      Validators = validators;
    }


    /// <summary>
    /// 表单字段验证器列表
    /// </summary>
    protected FormFieldValidatorCollection Validators { get; private set; }


    /// <summary>
    /// 对表单执行验证
    /// </summary>
    /// <param name="form">要验证的表单</param>
    /// <returns>验证结果</returns>
    public IFormValidationResult ValidateForm( HtmlForm form )
    {

      return new FormValidationResult( form, form.Controls.Select( control => Validators[control.Name].Validate( control.Value ) ).NotNull() );

    }
  }
}
