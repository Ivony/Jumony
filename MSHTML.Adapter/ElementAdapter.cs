using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public class ElementAdapter : NodeAdapter, IHtmlElement
  {


    private object _raw;
    private IHTMLElement _element;
    private IHTMLElement2 _element2;
    private IHTMLElement3 _element3;
    private IHTMLElement4 _element4;


    public ElementAdapter( object element )
      : base( element )
    {

      _raw = element;

      _element = element as IHTMLElement;
      _element2 = element as IHTMLElement2;
      _element3 = element as IHTMLElement3;
      _element4 = element as IHTMLElement4;
    }


    public string Name
    {
      get { return _element.tagName; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      throw new NotImplementedException();
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      throw new NotImplementedException();
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }


    public override string RawHtml
    {
      get { return _element.outerHTML; }
    }
  }
}
