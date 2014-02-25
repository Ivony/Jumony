using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 表单控件组具体项的抽象
  /// </summary>
  public abstract class FormGroupControlItem
  {

    /// <summary>
    /// 创建表单空间组具体项的实例
    /// </summary>
    /// <param name="group">所属的控件组</param>
    protected FormGroupControlItem( FormGroupControlBase group )
    {
      GroupControl = group;
    }


    /// <summary>
    /// 获取所属的控件组
    /// </summary>
    public FormGroupControlBase GroupControl
    {
      get;
      private set;
    }



    /// <summary>
    /// 该项所代表的值
    /// </summary>
    public abstract string Value
    {
      get;
    }


    /// <summary>
    /// 该项是否处于被选中状态
    /// </summary>
    public abstract bool Selected
    {
      get;
      set;
    }



  }
}
