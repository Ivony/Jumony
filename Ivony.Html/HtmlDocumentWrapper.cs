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


    /// <summary>
    /// 被包装的 IHtmlDocument对象
    /// </summary>
    protected abstract IHtmlDocument Document
    {
      get;
    }




    string IHtmlDocument.DocumentDeclaration
    {
      get { return Document.DocumentDeclaration; }
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

    object IHtmlContainer.SyncRoot
    {
      get { return Document.SyncRoot; }
    }

    string IHtmlDomObject.RawHtml
    {
      get { return Document.RawHtml; }
    }


    /*
    public IHtmlContainer Container
    {
      get { return Document.Container; }
    }

    public void Remove()
    {
      Document.Remove();
    }
    */


    Uri IHtmlDocument.DocumentUri
    {
      get { return Document.DocumentUri; }
    }



    IHtmlFragmentManager IHtmlDocument.FragmentManager
    {
      get { return Document.FragmentManager; }
    }


    IHtmlDomModifier IHtmlDocument.DomModifier
    {
      get { return Document.DomModifier; }
    }


    HtmlSpecificationBase IHtmlDocument.HtmlSpecification
    {
      get { return Document.HtmlSpecification; }
    }
  }
}
