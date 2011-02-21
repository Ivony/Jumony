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
    public static T AddNode<T>( this IDomContainer container, T node ) where T : DomNode
    {
      container.NodeCollection.Add( node );
      return node;
    }
  }
}
