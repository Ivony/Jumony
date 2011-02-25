using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Forms.Validation
{
  public class IntegerValidator : IHtmlValueValidator
  {

    private static readonly Regex integerRegex = new Regex( @"^(\+|\-)?" + Regulars.integerPattern + "$" );

    #region IHtmlValueValidator 成员

    public bool Validate( string value )
    {
      return integerRegex.IsMatch( value );
    }

    public string ErrorMessage
    {
      get { return "<field>无法被识别为有效的整数"; }
    }

    public string RuleDescription
    {
      get { return "只能输入整数"; }
    }

    public string[] ValidExamples
    {
      get { return new string[] { "54221", "-3", "0" }; }
    }

    public string[] InvalidExamples
    {
      get { return new string[] { "1.5", "3.0" }; }
    }

    #endregion
  }
}
