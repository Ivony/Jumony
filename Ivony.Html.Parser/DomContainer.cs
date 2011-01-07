using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  public abstract class DomContainer : DomNode, IHtmlContainer, IDomContainer
  {

    protected DomContainer()
    {
      collection = new DomNodeCollection( this );
    }

    private DomNodeCollection collection;

    public IEnumerable<IHtmlNode> Nodes()
    {
      return collection.Cast<IHtmlNode>().AsReadOnly();
    }

    public DomNodeCollection NodeCollection
    {
      get { return collection; }
    }
  }
}
