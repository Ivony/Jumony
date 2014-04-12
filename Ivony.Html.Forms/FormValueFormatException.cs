using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 当设置的表单值格式不正确时抛出的异常
  /// </summary>
  public class FormValueFormatException : FormatException
  {

    /// <summary>
    /// 设置错误格式值的表单控件
    /// </summary>
    public IFormControl Control
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建 FormValueFormatException 对象
    /// </summary>
    /// <param name="control">设置错误值的表单控件</param>
    /// <param name="message">异常消息</param>
    public FormValueFormatException( IFormControl control, string message )
      : base( message )
    {
      Control = control;
    }


  }
}
