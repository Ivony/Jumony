using System;
namespace Ivony.Html.Forms
{
  public interface IFormValidationResult
  {

    IFormValidator Validator { get; }

    bool HasError { get; }


    FormValidationErrorCollection Errors { get; }
  }
}
