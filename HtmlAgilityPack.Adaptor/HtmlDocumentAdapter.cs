using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;
using Ivony.Fluent;
using System.Text.RegularExpressions;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlDocumentAdapter : HtmlContainerAdapter, IHtmlDocument
  {


    private Uri _url;
    private AP.HtmlDocument _document;

    public HtmlDocumentAdapter( AP.HtmlDocument document )
      : base( document.DocumentNode )
    {

      if ( document.DocumentNode.ChildNodes.Any() )
      {
        var node = document.DocumentNode.ChildNodes[0];

        if ( node.NodeType == AP.HtmlNodeType.Comment )
        {
          if ( node.InnerHtml.StartsWith( "<!DTD" ) )
            _declaration = node.InnerHtml;
        }
      }

      _document = document;

      var uriAttribute = document.DocumentNode.Attributes.SingleOrDefault( a => a.Name == "uri" );
      if ( uriAttribute != null )
      {
        Uri.TryCreate( uriAttribute.Value, UriKind.Absolute, out _url );
      }

    }

    string _declaration;

    public string DocumentDeclaration
    {
      get { return _declaration; }
    }


    public Uri DocumentUri
    {
      get { return _url; }
    }



    public IHtmlNodeFactory GetNodeFactory()
    {
      return new HtmlNodeFactory( this._document );
    }


    void IHtmlNode.Remove()
    {
      throw new NotSupportedException();
    }

    IHtmlDocument IHtmlDomObject.Document
    {
      get { return this; }
    }

  }
}
