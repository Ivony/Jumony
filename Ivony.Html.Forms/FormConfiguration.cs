using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  
  public class FormConfiguration
  {
    /// <summary>
    /// 发现控件的必要属性设置了错误的值时，是否应当抛出异常
    /// </summary>
    public bool ExceptionOnAttributeError { get; set; }

    /// <summary>
    /// 给单行文本框设置多行文本值，是否直接忽略所有换行符
    /// </summary>
    public bool IgnoreNewline { get; set; }

    /// <summary>
    /// 给文本输入控件设置值时，是否应当检查设置的值超出了 maxlength 属性所允许的长度。
    /// </summary>
    public bool CheckMaxLength { get; set; }
  }
}
