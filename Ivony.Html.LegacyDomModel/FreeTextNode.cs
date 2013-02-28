using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  internal class FreeTextNode : FreeNode, IFreeTextNode
  {
    internal FreeTextNode( IHtmlFragment fragment, IHtmlNodeFactory factory )
      : base( fragment, factory )
    {
      TextNode = Node as IHtmlTextNode;
      if ( TextNode == null )
        throw new InvalidOperationException();
    }


    private IHtmlTextNode TextNode
    {
      get;
      set;
    }


    public string HtmlText
    {
      get { return TextNode.HtmlText; }
    }
  }
}
