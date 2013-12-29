using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 将被绑定元素复制若干份以绑定列表的绑定上下文
  /// </summary>
  public sealed class HtmlRepeatBindingContext : HtmlBindingContext
  {

    internal HtmlRepeatBindingContext( HtmlBindingContext context, IHtmlElement element, IEnumerable dataContext )
      : base( context, element, dataContext )
    {
      _element = element;
      _dataList = dataContext.Cast<object>().ToArray();
    }


    private IHtmlElement _element;
    private object[] _dataList;


    /// <summary>
    /// 重写 DataBind 方法，执行绑定时将被绑定元素复制适当份数，再进行绑定。
    /// </summary>
    public override void DataBind()
    {
      var count = _dataList.Length;
      var list = _element.Repeat( count );

      for( int i = 0; i < count; i++ )
        CreateBindingContext( ParentContext, list[i], _dataList[i] ).DataBind();
    }

  }
}
