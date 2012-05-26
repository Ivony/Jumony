using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingProvider
  {
    IBinding CreateBinding( IHtmlDomObject target, IDictionary<string, string> args );
  }

}
