using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IValueBinder
  {

    void BindValue( IHtmlDomObject target, object value );

    Type ValueType { get; }

  }
}
