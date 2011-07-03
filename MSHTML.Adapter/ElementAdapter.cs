using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public class ElementAdapter : NodeAdapter, IHtmlElement
  {


    private IHTMLElement _element;
    private IHTMLElement2 _element2;
    private IHTMLElement3 _element3;
    private IHTMLElement4 _element4;


    public ElementAdapter( IHTMLElement element )
      : base( (IHTMLDOMNode) element )
    {
      _element = element;

      _element2 = element as IHTMLElement2;
      _element3 = element as IHTMLElement3;
      _element4 = element as IHTMLElement4;
    }


    #region IHtmlElement 成员

    public string Name
    {
      get { return _element.tagName; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IHtmlContainer 成员

    public IEnumerable<IHtmlNode> Nodes()
    {
      throw new NotImplementedException();
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    #endregion

    public override string RawHtml
    {
      get { return _element.outerHTML; }
    }
  }
}
