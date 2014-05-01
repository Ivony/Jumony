using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表单控件容器
  /// </summary>
  public sealed class FormControlCollection : KeyedCollection<string, IFormControl>
  {
    /// <summary>
    /// 创建 FormControlCollection 对象
    /// </summary>
    public FormControlCollection() { }


    /// <summary>
    /// 创建 FormControlCollection 对象
    /// </summary>
    /// <param name="controls">要添加的控件</param>
    public FormControlCollection( IFormControl[] controls )
      : this()
    {
      foreach ( var c in controls )
        Add( c );
    }

    /// <summary>
    /// 重写 GetKeyForItem 方法，从表单控件中提取 Name （控件名称）属性
    /// </summary>
    /// <param name="item">要提取名称的表单控件</param>
    /// <returns>表单控件的名称</returns>
    protected override string GetKeyForItem( IFormControl item )
    {
      return item.Name;
    }


    /// <summary>
    /// 获取所有控件的名称
    /// </summary>
    public string[] ControlNames
    {
      get { return Dictionary.Keys.ToArray(); }
    }

  }
}
