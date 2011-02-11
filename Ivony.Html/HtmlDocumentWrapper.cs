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




    string IHtmlDocument.DocumentDeclaration
    {
      get { return Document.DocumentDeclaration; }
    }

    IHtmlNodeFactory IHtmlDocument.GetNodeFactory()
    {
      return Document.GetNodeFactory();
    }


    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Document.Nodes();
    }


    object IHtmlDomObject.RawObject
    {
      get { return Document.RawObject; }
    }

    IHtmlDocument IHtmlDomObject.Document
    {
      get { return Document.Document; }
    }

    object IHtmlDomObject.SyncRoot
    {
      get { return Document.SyncRoot; }
    }


    public IHtmlContainer Container
    {
      get { return Document.Container; }
    }

    public string RawHtml
    {
      get { return Document.RawHtml; }
    }

    public void Remove()
    {
      Document.Remove();
    }

    public Uri DocumentUri
    {
      get { return Document.DocumentUri; }
    }

  }
}
