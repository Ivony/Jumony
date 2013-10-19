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
  }
}
