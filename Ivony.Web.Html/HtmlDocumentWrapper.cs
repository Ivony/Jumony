using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{

  /// <summary>
  /// IHtmlDocument的包裹类
  /// </summary>
  public abstract class HtmlDocumentWrapper : IHtmlDocument
  {

    protected abstract IHtmlDocument Document
    {
      get;
    }


    #region IHtmlDocument 成员

    string IHtmlDocument.DocumentDeclaration
    {
      get { return Document.DocumentDeclaration; }
    }

    string IHtmlDocument.Handle( IHtmlNode node )
    {
      return Document.Handle( node );
    }

    IHtmlNode IHtmlDocument.Handle( string handler )
    {
      return Document.Handle( handler );
    }

    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Document.Nodes();
    }

    #endregion

    #region IHtmlNode 成员

    IHtmlContainer IHtmlNode.Parent
    {
      get { return Document.Parent; }
    }

    object IHtmlNode.NodeObject
    {
      get { return Document.NodeObject; }
    }

    void IHtmlNode.Remove()
    {
      Document.Remove();
    }

    IHtmlDocument IHtmlNode.Document
    {
      get { return Document; }
    }

    string IHtmlNode.RawHtml
    {
      get { return Document.RawHtml; }
    }

    #endregion



    public override int GetHashCode()
    {
      return Document.GetHashCode();
    }

    public override bool Equals( object obj )
    {
      return Document.Equals( obj );
    }

  }
}
