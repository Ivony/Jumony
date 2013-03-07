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

    }


    public string Name
    {
      get { return _element.tagName; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      return new AttributeCollection( this );
    }


    protected class AttributeCollection : IHtmlAttributeCollection
    {

      private IHTMLAttributeCollection _attributes;
      private IHTMLAttributeCollection2 _attributes2;

      private ElementAdapter _element;
      public AttributeCollection( ElementAdapter element )
      {
        _attributes = element._node.attributes as IHTMLAttributeCollection;
        _attributes2 = element._node.attributes as IHTMLAttributeCollection2;
      }


      public IHtmlAttribute Get( string name )
      {
        return ConvertExtensions.AsAttribute( _attributes.item( name ), _element );
      }

      public IEnumerator<IHtmlAttribute> GetEnumerator()
      {
        return _attributes.Cast<object>().Select( o => ConvertExtensions.AsAttribute( o, _element ) ).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }
    }



    public IEnumerable<IHtmlNode> Nodes()
    {
      return ( (IEnumerable) _node.childNodes ).Cast<object>().Select( o => ConvertExtensions.AsNode( o ) );
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
