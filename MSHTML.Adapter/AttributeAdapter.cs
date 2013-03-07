using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public class AttributeAdapter : IHtmlAttribute
  {

    private object _raw;

    private IHTMLDOMAttribute _attribute;
    private IHTMLDOMAttribute2 _attribute2;

    private IHtmlElement _element;

    public AttributeAdapter( object attribute, IHtmlElement element )
    {
      _attribute = attribute as IHTMLDOMAttribute;
      _attribute2 = attribute as IHTMLDOMAttribute2;

      _element = element;

      _raw = attribute;
    }




    public IHtmlElement Element
    {
      get { return _element; }
    }

    public string Name
    {
      get { return _attribute.nodeName; }
    }

    public string AttributeValue
    {
      get { return _attribute.nodeValue as string; }
    }

    public object RawObject
    {
      get { return _raw; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public IHtmlDocument Document
    {
      get { return Element.Document; }
    }
  }
}
