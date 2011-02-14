using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// IHtmlNode 的包裹类
  /// </summary>
  public abstract class HtmlNodeWrapper : IHtmlNode
  {

    protected abstract IHtmlNode Node
    {
      get;
    }

    #region IHtmlNode 成员

    IHtmlContainer IHtmlNode.Container
    {
      get { return Node.Container; }
    }

    object IHtmlDomObject.RawObject
    {
      get { return Node.RawObject; }
    }

    void IHtmlNode.Remove()
    {
      Node.Remove();
    }

    IHtmlDocument IHtmlDomObject.Document
    {
      get { return Node.Document; }
    }

    string IHtmlDomObject.RawHtml
    {
      get { return Node.RawHtml; }
    }

    object IHtmlDomObject.SyncRoot
    {
      get { return Node.SyncRoot; }
    }

    #endregion


    public override int GetHashCode()
    {
      return Node.GetHashCode();
    }

    public override bool Equals( object obj )
    {
      return Node.Equals( obj );
    }


  }
}
