using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  public interface IElementBinder
  {

    void BindElement( BindingContext context, IHtmlElement element );

  }
}
