using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  internal sealed class FreeElement : FreeNode, IFreeElement
  {

    internal FreeElement( IHtmlFragment fragment, IHtmlNodeFactory factory )
      : base( fragment, factory )
    {
      Element = Node as IHtmlElement;
      if ( Element == null )
        throw new InvalidOperationException();
    }


    private IHtmlElement Element
    {
      get;
      set;
    }


    public string Name
    {
      get { return Element.Name; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      return Element.Attributes();
    }

    public IEnumerable<IHtmlNode> Nodes()
    {
      return Element.Nodes();
    }

    public object SyncRoot
    {
      get { return Element.SyncRoot; }
    }
  }
}
