using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using mshtml;


namespace Ivony.Html.MSHTMLAdapter
{
  public abstract class NodeAdapter : IHtmlNode
  {

    private IHTMLDOMNode _node;

    public NodeAdapter( IHTMLDOMNode node )
    {
      _node = node;
    }

    #region IHtmlNode 成员

    public IHtmlContainer Container
    {
      get { throw new NotImplementedException(); }
    }

    #endregion

    #region IHtmlDomObject 成员

    public object RawObject
    {
      get { return _node; }
    }

    public abstract string RawHtml
    {
      get;
    }

    public IHtmlDocument Document
    {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}
