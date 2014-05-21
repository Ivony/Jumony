using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  internal class HtmlBindingContextProviderCollection : SynchronizedKeyedCollection<Type, IHtmlBindingContextProvider>
  {
    protected override Type GetKeyForItem( IHtmlBindingContextProvider item )
    {
      var type = item.ModelType;

      if ( type.IsInterface )
        throw new InvalidOperationException();

      if ( type == typeof( object ) )
        throw new InvalidOperationException();


      return type;
    }



  }
}
