using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public interface IDomContainer : IHtmlContainer
  {
    DomNodeCollection NodeCollection
    {
      get;
    }

  }

  internal static class DomContianerExtension
  {
    public static T InsertNode<T>( this IDomContainer container, int index, T node ) where T : DomNode
    {
      container.NodeCollection.Insert( index, node );
      return node;
    }
  }
}
