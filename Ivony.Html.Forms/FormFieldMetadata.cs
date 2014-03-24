using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单字段元数据
  /// </summary>
  public class FormFieldMetadata
  {

    /// <summary>
    /// 字段名称
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// 字段显示名称
    /// </summary>
    public string DisplayName { get; set; }


    /// <summary>
    /// 字段描述
    /// </summary>
    public string FieldDescription { get; set; }

    /// <summary>
    /// 填写规则描述
    /// </summary>
    public string RuleDescription { get; set; }

  }
}
