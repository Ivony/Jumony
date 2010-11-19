using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// IHtmlDocument 的包裹类
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

    IHtmlNodeFactory IHtmlDocument.GetNodeFactory()
    {
      return Document.GetNodeFactory();
    }

    #endregion

    #region IHtmlNodeContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Document.Nodes();
    }

    #endregion

    #region IHtmlObject 成员

    object IHtmlObject.RawObject
    {
      get { return Document.RawObject; }
    }

    IHtmlDocument IHtmlObject.Document
    {
      get { return this; }
    }

    object IHtmlObject.SyncRoot
    {
      get { return Document.SyncRoot; }
    }

    #endregion
  }
}
