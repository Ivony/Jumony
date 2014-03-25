using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 定义表单字段验证规则
  /// </summary>
  public interface IFormValidationRule
  {


    /// <summary>
    /// 指定值是否满足验证规则
    /// </summary>
    /// <param name="value">指定的值</param>
    /// <returns>是否满足规则</returns>
    bool IsValid( string value );


    /// <summary>
    /// 获取错误信息模板
    /// </summary>
    string ErrorMessageTemplate { get; }


    /// <summary>
    /// 获取规则描述
    /// </summary>
    string RuleDescription { get; }

  }
}
