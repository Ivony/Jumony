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
    }

    string _declaration;

    public string DocumentDeclaration
    {
      get { return _declaration; }
    }


    public string Handle( IHtmlNode node )
    {
      var htmlNode = node.NodeObject.CastTo<AP.HtmlNode>();
      return string.Format( "{0}:{1}", htmlNode.Line, htmlNode.LinePosition );
    }


    Regex handleRegex = new Regex( string.Format( "^{0}:{1}$", Regulars.integerPattern, Regulars.integerPattern ), RegexOptions.Compiled );

    public IHtmlNode Handle( string handler )
    {
      if ( handleRegex.IsMatch( handler ) )
        throw new FormatException();

      string[] values = handler.Split( ':' );
      int line = int.Parse( values[0] );
      int linePosition = int.Parse( values[1] );

      var htmlNode = Node.DescendantNodesAndSelf().FirstOrDefault( node => node.Line == line && node.LinePosition == linePosition );
      if ( htmlNode == null )
        return null;

      return htmlNode.AsNode();

    }

    public IHtmlNodeFactory GetNodeFactory()
    {
      return new HtmlNodeFactory( this._document );
    }




    void IHtmlNode.Remove()
    {
      throw new InvalidOperationException();
    }

    IHtmlDocument IHtmlNode.Document
    {
      get { return this; }
    }

  }
}
