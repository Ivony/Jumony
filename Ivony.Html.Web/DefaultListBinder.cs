using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{
  public sealed class DefaultListBinder : IHtmlListBinder
  {
    public bool BindList( IHtmlElement element, HtmlListBindingContext bindingContext )
    {


    }


    public virtual bool BindAttribute( HtmlBindingContext context, IHtmlAttribute attribute )
    {
      return false;
    }

    public virtual bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      return false;
    }
  }
}
