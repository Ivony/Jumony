using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;
using Ivony.Html.Forms;

namespace Ivony.Html.Web.Mvc
{
  public static class MvcFormExtensions
  {

    public static HtmlForm ApplyValues( this HtmlForm form, object dataModel )
    {

      var data = dataModel.ToPropertiesMap();

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {

        if ( data.ContainsKey( key ) )
          form[key].TrySetValue( data[key] );

      }

      return form;

    }


    public static HtmlForm ApplyValues( this HtmlForm form, IValueProvider valueProvider )
    {

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {
        if ( valueProvider.ContainsPrefix( key ) )
        {
          form[key].TrySetValue( valueProvider.GetValue( key ).AttemptedValue );
        }
      }

      return form;

    }

  }
}
