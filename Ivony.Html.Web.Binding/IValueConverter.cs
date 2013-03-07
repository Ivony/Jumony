using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IValueConverter
  {

    object Convert( object value );

    Type ValueType { get; }

  }
}
