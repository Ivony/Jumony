using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{
  public class HtmlListBindingContext : HtmlBindingContext, IHtmlBindingContextProvider
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


    public HtmlBindingContext CreateBindingContext( HtmlBindingContext parentContext, IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues )
    {
      var dataList = dataContext as IEnumerable;
      if ( dataList == null )
        return null;

      var element = scope as IHtmlElement;
      if ( element == null )
        return null;

      if ( element.Name.EqualsIgnoreCase( "ul" ) || element.Name.EqualsIgnoreCase( "ol" ) )
      {
        if ( element.Elements().All( e => e.Name.EqualsIgnoreCase( "li" ) ) )
          return new HtmlListBindingContext( parentContext, element, dataList, dataValues );

      }

      return null;

    }
  }
}
