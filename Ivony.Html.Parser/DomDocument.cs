using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  public class DomDocument : DomObject, IHtmlDocument, IDomContainer
  {

    public DomDocument()
    {
      nodeCollection = new DomNodeCollection( this );
    }

    public override IHtmlDocument Document
    {
      get { return this; }
    }

    public string DocumentDeclaration
    {
      get { return null; }
    }

    public IHtmlNodeFactory GetNodeFactory()
    {
      return new DomFactory( this );
    }

    private readonly DomNodeCollection nodeCollection;

    DomNodeCollection IDomContainer.NodeCollection
    {
      get { return nodeCollection; }
    }

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return nodeCollection.Cast<IHtmlNode>().AsReadOnly();
    }


    public IHtmlContainer Container
    {
      get { return null; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public void Remove()
    {
      throw new NotSupportedException();
    }
  }
}
