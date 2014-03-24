using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示分析、设置表单控件时发生的异常
  /// </summary>
  public class FormControlException : Exception
  {

    /// <summary>
    /// 创建 FromControlException 对象
    /// </summary>
    /// <param name="control">引发异常的表单控件</param>
    /// <param name="message">异常信息</param>
    public FormControlException( IFormControl control, string message )
      : base( message )
    {
      Control = control;
    }


    /// <summary>
    /// 引发异常的表单控件
    /// </summary>
    public IFormControl Control
    {
      get;
      private set;
    }

  }
}
