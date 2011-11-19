using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using System.Collections;

namespace Ivony.Html.MSHTMLAdapter
{
  public class ElementAdapter : NodeAdapter, IHtmlElement
  {


    private object _raw;
    private IHTMLElement _element;
    private IHTMLElement2 _element2;
    private IHTMLElement3 _element3;
    private IHTMLElement4 _element4;

    private IHTMLAttributeCollection _attributes;
    private IHTMLAttributeCollection2 _attributes2;


    public ElementAdapter( object element )
      : base( element )
    {

      _raw = element;

      _element = element as IHTMLElement;
      _element2 = element as IHTMLElement2;
      _element3 = element as IHTMLElement3;
      _element4 = element as IHTMLElement4;

      _attributes = _node.attributes as IHTMLAttributeCollection;
      _attributes2 = _node.attributes as IHTMLAttributeCollection2;
    }


    public string Name
    {
      get { return _element.tagName; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      return _attributes.Cast<object>().Select( o => ConvertExtensions.AsAttribute( o, this ) );
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      return ( (IEnumerable) _element.children ).Cast<object>().Select( o => ConvertExtensions.AsNode( o ) );
    }


    private object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }


    public override string RawHtml
    {
      get { return _element.outerHTML; }
    }
  }
}
