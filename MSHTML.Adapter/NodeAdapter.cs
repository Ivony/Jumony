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
    protected readonly IHTMLDOMNode _node;
    protected readonly IHTMLDOMNode2 _node2;

    public NodeAdapter( object node )
    {
      _raw = node;

      _node = node as IHTMLDOMNode;
      _node2 = node as IHTMLDOMNode2;
    }


    public IHtmlContainer Container
    {
      get
      {
        var parent = _node.parentNode;
        if ( parent != null )
          return new ElementAdapter( _node.parentNode );

        else
          return Document;
      }
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
      get { return new DocumentAdapter( _node2.ownerDocument ); }
    }

  }
}
