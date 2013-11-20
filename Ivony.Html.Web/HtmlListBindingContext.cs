using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 进行列表数据绑定的时候的绑定上下文信息
  /// </summary>
  public sealed class HtmlListBindingContext : HtmlBindingContext
  {

    internal HtmlListBindingContext( HtmlBindingContext context, IHtmlElement element, IListDataContext dataContext )
      : base( context, element, dataContext.ListData )
    {
      ListDataContext = dataContext;
    }


    public IListDataContext ListDataContext
    {
      private set;
      get;
    }


    private bool IsListItem( IHtmlElement element )
    {
      return element.Name.EqualsIgnoreCase( "li" ) || element.Name.EqualsIgnoreCase( "tr" ) || element.Name.EqualsIgnoreCase( "view" ) || element.Name.EqualsIgnoreCase( "binding" );
    }

  }
}
