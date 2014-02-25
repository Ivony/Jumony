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
    /// 给组输入控件设置无效的值（Value）时，是否应当抛出异常
    /// </summary>
    public bool ExceptionOnInvailidValues { get; set; }

    /// <summary>
    /// 给文本输入控件设置的值超出了 maxlength 的限制时，是否应当抛出异常。
    /// </summary>
    public bool ExceptionOnOverflowOfLength { get; set; }



    /// <summary>
    /// 给单行文本框设置多行文本值，是否直接忽略所有换行符
    /// </summary>
    public bool IgnoreNewline { get; set; }



    /// <summary>
    /// 获取默认的表单设置
    /// </summary>
    public static FormConfiguration Default
    {
      get { return new FormConfiguration(); }
    }


  }
}
