using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义用于呈现表单验证信息的呈现器
  /// </summary>
  public interface IFormValidationPresenter
  {

    /// <summary>
    /// 显示表单验证结果
    /// </summary>
    /// <param name="result"></param>
    void ShowValidationResult( IFormValidationResult result );

  }
}
