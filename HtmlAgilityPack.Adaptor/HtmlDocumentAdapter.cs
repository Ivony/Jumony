using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;
using Ivony.Fluent;
using System.Text.RegularExpressions;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlDocumentAdapter : IHtmlDocument, IHtmlContainerNode
  {



    private AP.HtmlDocument _document;

    public HtmlDocumentAdapter( AP.HtmlDocument document )
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
    }

    string _declaration;

    public string DocumentDeclaration
    {
      get { return _declaration; }
    }


    public IHtmlNodeFactory GetNodeFactory()
    {
      return new HtmlNodeFactory( this._document );
    }


    IHtmlDocument IHtmlDomObject.Document
    {
      get { return this; }
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      return ChildNodes.Select( node => node.AsNode() );
    }

    public AP.HtmlNode Node
    {
      get { return _document.DocumentNode; }
    }

    public object RawObject
    {
      get { return Node; }
    }

    public object SyncRoot
    {
      get { return Node; }
    }


    public AP.HtmlNodeCollection ChildNodes
    {
      get { return Node.ChildNodes; }
    }


  }
}
