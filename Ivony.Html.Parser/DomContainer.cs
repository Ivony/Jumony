using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;


namespace Ivony.Html.Parser
{
  public abstract class DomContainer : DomNode, IDomContainer
  {

    protected DomContainer()
    {
      _nodes = new DomNodeCollection( this );
    }



    private readonly DomNodeCollection _nodes;

    #region IDomContainer 成员

    DomNodeCollection IDomContainer.NodeCollection
    {
      get { return _nodes; }
    }

    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return _nodes.Cast<IHtmlNode>();
    }

    #endregion

  }
}
