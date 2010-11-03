using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms.Validation
{
  public interface IHtmlValueValidator
  {

    bool Validate( string value );

    string GenerateScript();

    string ErrorMessage { get; }

    string RuleDescription { get; }

  }
}
