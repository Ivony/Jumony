using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 进行列表绑定的绑定上下文
  /// </summary>
  internal sealed class HtmlListBindingContext : HtmlBindingContext
  {

    internal HtmlListBindingContext( HtmlBindingContext context, IHtmlElement element, ListDataModel dataContext )
      : base( context, element, dataContext )
    {
      _element = element;
      _dataContext = dataContext;
    }


    private IHtmlElement _element;
    private ListDataModel _dataContext;


    /// <summary>
    /// 重写 DataBind 方法，执行绑定时将被绑定元素复制适当份数，再进行绑定。
    /// </summary>
    public override void DataBind()
    {

      if ( _dataContext.BindingMode != ListBindingMode.Repeat )
        throw new NotSupportedException();//暂不支持 Repeat 之外的绑定方式

      var count = _dataContext.ListData.Length;
      var list = _element.Repeat( count );

      for ( int i = 0; i < count; i++ )
        CreateBindingContext( ParentContext, list[i], _dataContext.ListData[i] ).DataBind();
    }

  }
}
