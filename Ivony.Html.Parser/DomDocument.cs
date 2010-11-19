using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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



    #region IDomContianer Implements

    private readonly DomNodeCollection nodeCollection;

    DomNodeCollection IDomContainer.NodeCollection
    {
      get { return nodeCollection; }
    }

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return nodeCollection.Cast<IHtmlNode>();
    }

    #endregion

  }
}
