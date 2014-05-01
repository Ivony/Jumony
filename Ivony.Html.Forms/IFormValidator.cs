using System;
namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单验证器
  /// </summary>
  public interface IFormValidator
  {

    /// <summary>
    /// 验证表单
    /// </summary>
    /// <param name="form">要验证的表单</param>
    /// <returns>表单验证结果</returns>
    IFormValidationResult ValidateForm( HtmlForm form );
  }
}
