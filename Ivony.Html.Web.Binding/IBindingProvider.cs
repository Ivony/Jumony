using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingProvider
  {
    IBinding CreateBinding( BindingManager manager, IHtmlDomObject targetObject, IDictionary<string, string> args );
  }

}
