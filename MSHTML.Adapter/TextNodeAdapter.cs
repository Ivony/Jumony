using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public class TextNodeAdapter : NodeAdapter, IHtmlTextNode
  {


    private IHTMLDOMTextNode _textNode;
    private IHTMLDOMTextNode2 _textNode2;

    public TextNodeAdapter( object node )
      : base( node )
    {
      _textNode = node as IHTMLDOMTextNode;
      _textNode2 = node as IHTMLDOMTextNode2;

    }


    public override string RawHtml
    {
      get { return null; }
    }

    public string HtmlText
    {
      get { return _textNode.data; }
    }
  }
}
