using System;
namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单验证结果
  /// </summary>
  public interface IFormValidationResult
  {


    /// <summary>
    /// 所属表单
    /// </summary>
    HtmlForm Form { get; }



    /// <summary>
    /// 是否存在验证错误
    /// </summary>
    bool HasError { get; }


    /// <summary>
    /// 获取验证错误消息集合
    /// </summary>
    FormValidationErrorCollection Errors { get; }
  }
}
