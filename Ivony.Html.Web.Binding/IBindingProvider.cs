using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingProvider
  {
    IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlElement element );

    IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlDocument document );
  }



}
