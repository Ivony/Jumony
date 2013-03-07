using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// IHtmlElement 的包裹类
  /// </summary>
  public abstract class HtmlElementWrapper : HtmlNodeWrapper, IHtmlElement
  {


    /// <summary>
    /// 被包装的 IHtmlElement 对象
    /// </summary>
    protected abstract IHtmlElement Element
    {
      get;
    }

    #region IHtmlElement 成员

    string IHtmlElement.Name
    {
      get { return Element.Name; }
    }

    IEnumerable<IHtmlAttribute> IHtmlElement.Attributes()
    {
      return Element.Attributes();
    }

    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Element.Nodes();
    }

    object IHtmlContainer.SyncRoot
    {
      get { return Element.SyncRoot; }
    }

    #endregion

    /// <summary>
    /// 提供被包装的 IHtmlNode 对象，用于实现 HtmlNodeWrapper
    /// </summary>
    protected override sealed IHtmlNode Node
    {
      get { return Element; }
    }
  }
}
