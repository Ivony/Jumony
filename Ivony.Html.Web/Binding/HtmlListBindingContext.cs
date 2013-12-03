using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{
  public class HtmlListBindingContext : HtmlBindingContext
  {

    protected HtmlListBindingContext( HtmlBindingContext context, IHtmlContainer scope, IEnumerable dataList, IDictionary<string, object> dataValues )
      : base( context, scope, dataList, dataValues )
    {
      _dataList = dataList.Cast<object>().ToArray();
    }


    private object[] _dataList;

    protected override void BindChilds( IHtmlContainer container )
    {

      if ( container.Equals( BindingScope ) )
      {
        var count = _dataList.Length;
      }

      base.BindChilds( container );
    }
  }
}
