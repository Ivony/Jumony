using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单验证错误
  /// </summary>
  public class FormValidationError
  {

    /// <summary>
    /// 字段名称
    /// </summary>
    public string Name { get; private set; }


    /// <summary>
    /// 错误消息
    /// </summary>
    public string[] Messages { get; private set; }

  }
}
