using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  public interface IValueProvider
  {

    string GetValue( object data, IHtmlElement target, string path );

  }
}
