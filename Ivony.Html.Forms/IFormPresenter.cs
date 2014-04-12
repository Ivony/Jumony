using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义用于呈现表单元数据和验证信息的呈现器
  /// </summary>
  public interface IFormPresenter
  {

    /// <summary>
    /// 显示表单验证结果
    /// </summary>
    /// <param name="result">表单验证结果</param>
    void ShowValidationResult( IFormValidationResult result );


    /// <summary>
    /// 显示表单元数据
    /// </summary>
    /// <param name="metadata">表单元数据</param>
    void ShowMetadata( FormMetadata metadata );

  }
}
