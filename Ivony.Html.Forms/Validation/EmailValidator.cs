using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Forms.Validation
{
  public class EmailValidator : IHtmlValueValidator
  {

    private static readonly Regex emailRegex = new Regex( "^" + ValidationRegulars.emailPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    #region IHtmlValueValidator 成员

    public bool Validate( string value )
    {
      return emailRegex.IsMatch( value );
    }

    public string ErrorMessage
    {
      get { return "<field>不是一个有效的电子邮件地址。"; }
    }

    public string RuleDescription
    {
      get { return "只能输入有效的电子邮件地址"; }
    }

    public string[] ValidExamples
    {
      get { return new string[] { "Ivony@live.com" }; }
    }

    public string[] InvalidExamples
    {
      get { return new string[] { "microsoft.com", "anybody@nothing" }; }
    }

    #endregion
  }
}
