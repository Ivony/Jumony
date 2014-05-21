using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 定义一个 FormControlValidator 的容器
  /// </summary>
  public class FormFieldValidatorCollection : KeyedCollection<string, IFormFieldValidator>
  {


    /// <summary>
    /// 创建 FormControlValidatorCollection 对象
    /// </summary>
    public FormFieldValidatorCollection() : base( StringComparer.OrdinalIgnoreCase ) { }


    /// <summary>
    /// 创建 FormControlValidatorCollection 对象
    /// </summary>
    /// <param name="validators">字段验证器集合</param>
    public FormFieldValidatorCollection( IEnumerable<IFormFieldValidator> validators )
      : this()
    {
      foreach ( var v in validators )
      {
        Add( v );
      }
    }


    /// <summary>
    /// 重写 GetKeyForItem 方法，抽取验证的控件名
    /// </summary>
    /// <param name="item">要抽取控件名的控件验证器</param>
    /// <returns>控件名</returns>
    protected override string GetKeyForItem( IFormFieldValidator item )
    {
      return item.Name;
    }
  }
}
