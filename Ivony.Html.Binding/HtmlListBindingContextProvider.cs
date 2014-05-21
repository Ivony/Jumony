using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  internal sealed class HtmlListBindingContextProvider : IHtmlBindingContextProvider
  {
    public HtmlBindingContext CreateBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object datamodel )
    {
      return new HtmlListBindingContext( bindingContext, (IHtmlElement) scope, (ListDataModel) datamodel );
    }



    private readonly static Type _modelType = typeof( ListDataModel );

    public Type ModelType
    {
      get { return _modelType; }
    }
  }
}
