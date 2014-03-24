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
    /// <param name="validators">输入控件验证器列表</param>
    public FormValidator( FormFieldValidatorCollection validators )
    {
      Validators = validators;
    }


    protected FormFieldValidatorCollection Validators { get; private set; }

    public IFormValidationResult ValidateForm( HtmlForm form )
    {

      return new FormValidationResult( this, form.Controls.Select( control => Validators[control.Name].Validate( control.Value ) ).NotNull() );

    }
  }
}
