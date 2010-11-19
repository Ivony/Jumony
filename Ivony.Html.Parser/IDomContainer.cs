using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  internal interface IDomContainer : IHtmlContainer
  {
    DomNodeCollection NodeCollection
    {
      get;
    }

  }

  internal static class DomContianerExtension
  {
    public static DomNode InsertNode( this IDomContainer container, int index, DomNode node )
    {
      container.NodeCollection.Insert( index, node );
      return node;
    }
  }
}
