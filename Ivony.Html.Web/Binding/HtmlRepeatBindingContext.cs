using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 进行列表数据绑定的时候的绑定上下文信息
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


    public override void DataBind()
    {
      var count = _dataList.Length;
      var list = _element.Repeat( count );

      for( int i = 0; i < count; i++ )
        CreateBindingContext( ParentContext, list[i], _dataList[i] ).DataBind();
    }

  }
}
