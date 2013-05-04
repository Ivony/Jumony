using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;
using System.Collections;

namespace Ivony.Html.MSHTMLAdapter
{
  public class DocumentAdapter : IHtmlDocument
  {

    private Uri _documentUri;
    private object _raw;
    private IHTMLDocument _document;
    private IHTMLDocument2 _document2;
    private IHTMLDocument3 _document3;
    private IHTMLDocument4 _document4;
    private IHTMLDocument5 _document5;

    public DocumentAdapter( object document )
    {
      //_documentUri = documentUri;

      _raw = document;

      _document = document as IHTMLDocument;
      _document2 = document as IHTMLDocument2;
      _document3 = document as IHTMLDocument3;
      _document4 = document as IHTMLDocument4;
      _document5 = document as IHTMLDocument5;
    }


    public Uri DocumentUri
    {
      get { return _documentUri; }
    }

    public string DocumentDeclaration
    {
      get
      {
        if ( _document5 == null )
          return null;

        var doctype = _document5.doctype;
        if ( doctype == null )
          return null;

        return doctype.nodeValue;
      }
    }

    public IHtmlFragmentManager FragmentManager
    {
      get { throw new NotImplementedException(); }
    }

    public IHtmlDomModifier DomModifier
    {
      get { throw new NotImplementedException(); }
    }


    public IEnumerable<IHtmlNode> Nodes()
    {

      return ( (IEnumerable) _document3.childNodes ).Cast<object>().Select( o => ConvertExtensions.AsNode( o ) );
    }

    public object SyncRoot
    {
      get { return null; }
    }

    public object RawObject
    {
      get { return _raw; }
    }

    public string RawHtml
    {
      get
      {

        if ( _document3 == null )
          return null;

        return _document3.documentElement.outerHTML;
      }
    }

    public IHtmlDocument Document
    {
      get { return this; }
    }


    private HtmlSpecificationBase _specification = new Html41Specification();

    public HtmlSpecificationBase HtmlSpecification
    {
      get { return _specification; }
    }
  }
}
