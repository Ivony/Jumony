using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 定义 FormValidationError 对象的容器
  /// </summary>
  public class FormValidationErrorCollection : KeyedCollection<string, FormValidationError>
  {


    /// <summary>
    /// 创建 FormValidationErrorCollection 对象
    /// </summary>
    public FormValidationErrorCollection() : base( StringComparer.OrdinalIgnoreCase ) { }



    /// <summary>
    /// 重写 GetKeyForItem 方法抽取键值。
    /// </summary>
    /// <param name="item">要抽取键的 FormValidationError 对象</param>
    /// <returns>对象所对应的控件名</returns>
    protected override string GetKeyForItem( FormValidationError item )
    {
      return item.Name;
    }
  }
}
