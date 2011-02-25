using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms.Validation
{
  public class RequiredValidator : IHtmlValueValidator
  {
    #region IHtmlValueValidator 成员

    public bool Validate( string value )
    {
      if ( string.IsNullOrEmpty( value ) )
        return false;

      return true;
    }

    public string ErrorMessage
    {
      get { return "<field>不能为空。"; }
    }

    public string RuleDescription
    {
      get { return "必须填写"; }
    }

    public string[] ValidExamples
    {
      get { return null; }
    }

    public string[] InvalidExamples
    {
      get { return null; }
    }

    #endregion
  }
}
