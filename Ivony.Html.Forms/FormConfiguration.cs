using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 提供一些设置项，可以自定义 Jumony Forms 的行为。
  /// </summary>
  public sealed class FormConfiguration
  {

    /// <summary>
    /// 创建 FormCongfiguration 实例。
    /// </summary>
    public FormConfiguration()
    {
      IgnoreInvalidMaxlength = true;
    }


    /// <summary>
    /// 分析表单时是否忽略控件的 maxlength 属性设置的值格式错误。
    /// </summary>
    public bool IgnoreInvalidMaxlength { get; set; }


    /// <summary>
    /// 给输入组控件设置值超出了组控件的可选值范围时，是否忽略非法值的设置。
    /// </summary>
    public bool IgnoreInvailidValuesInGroupControl { get; set; }

    /// <summary>
    /// 给文本输入控件设置的值超出了 maxlength 的限制时，是否应当抛出异常。
    /// </summary>
    public bool IgnoreOverflowOfLength { get; set; }

    /// <summary>
    /// 给单行文本框设置多行文本值，是否直接忽略所有换行符
    /// </summary>
    public bool IgnoreNewlineInTextbox { get; set; }


    /// <summary>
    /// 分析表单时是否忽略输入控件设置的值不满足控件的限制的情况
    /// </summary>
    public bool IgnoreInvalidValueInTextControl
    {
      get { return _ignoreInvalidValueInTextControl; }
      set
      {
        if ( _readonly )
          throw new InvalidOperationException();
        
        _ignoreInvalidValueInTextControl = value;
      }
    }

    private bool _ignoreInvalidValueInTextControl;

    



    private bool _readonly = false;

    internal void MakeReadonly()
    {
      _readonly = true;
    }


  }
}
