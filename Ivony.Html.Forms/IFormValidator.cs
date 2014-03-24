using System;
namespace Ivony.Html.Forms
{
  public interface IFormValidator
  {
    IFormValidationResult ValidateForm( HtmlForm form );
  }
}
