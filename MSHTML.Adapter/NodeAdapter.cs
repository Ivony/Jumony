using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using mshtml;


namespace Ivony.Html.MSHTMLAdapter
{
  public abstract class NodeAdapter : IHtmlNode
  {

    private object _raw;
    private IHTMLDOMNode _node;
    private IHTMLDOMNode2 _node2;

    public NodeAdapter( object node )
    {
      _raw = node;

      _node = node as IHTMLDOMNode;
      _node2 = node as IHTMLDOMNode2;
    }


    public IHtmlContainer Container
    {
      get { throw new NotImplementedException(); }
    }


    public object RawObject
    {
      get { return _raw; }
    }

    public abstract string RawHtml
    {
      get;
    }

    public IHtmlDocument Document
    {
      get { throw new NotImplementedException(); }
    }

  }
}
