using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Forms.Validation
{

  /// <summary>
  /// 定义一个 HTML 输入值验证器
  /// </summary>
  public interface IHtmlValueValidator
  {

    bool Validate( string value );

    string ErrorMessage { get; }

    string RuleDescription { get; }

    string[] ValidExamples { get; }
    string[] InvalidExamples { get; }

  }
}
