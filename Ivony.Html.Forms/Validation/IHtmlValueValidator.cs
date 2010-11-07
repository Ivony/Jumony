using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Forms.Validation
{
  public interface IHtmlValueValidator
  {

    bool Validate( string value );

    string GenerateScript();

    string ErrorMessage { get; }

    string RuleDescription { get; }

    string[] ValidExamples { get; }
    string[] InvalidExamples { get; }

  }




}
