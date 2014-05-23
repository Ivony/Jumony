using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{


  /// <summary>
  /// 定义 IHtmlElementBinder 的容器
  /// </summary>
  public sealed class HtmlElementBinderCollection : KeyedCollection<string, IHtmlElementBinder>
  {

    /// <summary>
    /// 创建 HtmlElementBinderCollection 对象
    /// </summary>
    internal HtmlElementBinderCollection() : base( StringComparer.OrdinalIgnoreCase ) { }

    /// <summary>
    /// 创建 HtmlElementBinderCollection 对象
    /// </summary>
    /// <param name="binders">要添加的绑定器</param>
    internal HtmlElementBinderCollection( IEnumerable<IHtmlElementBinder> binders )
      : this()
    {
      foreach ( var item in binders )
        Add( item );
    }

    /// <summary>
    /// 从 IHtmlElementBinder 对象中提取用作键的元素名称
    /// </summary>
    /// <param name="item">IHtmlElementBinder 对象</param>
    /// <returns>用作键的元素名称</returns>
    protected override string GetKeyForItem( IHtmlElementBinder item )
    {
      return item.ElementName;
    }
  }
}
