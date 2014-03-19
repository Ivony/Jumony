using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 代表一个表单控件
  /// </summary>
  public abstract class FormControlBase
  {

    /// <summary>
    /// 创建 FormControl 对象
    /// </summary>
    /// <param name="form"></param>
    protected FormControlBase( HtmlForm form )
    {
      Form = form;
    }


    /// <summary>
    /// 控件所属的表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }


    /// <summary>
    /// 控件名
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 获取或设置控件目前的值
    /// </summary>
    public abstract string Value { get; set; }


    /// <summary>
    /// 检查值是否可以设置到控件
    /// </summary>
    /// <param name="value">要设置的值</param>
    /// <returns>是否可以设置</returns>
    public abstract bool CanSetValue( string value );


  }
}
