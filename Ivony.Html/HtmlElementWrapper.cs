using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// IHtmlElement 的包裹类
  /// </summary>
  public abstract class HtmlElementWrapper : HtmlNodeWrapper, IHtmlElement, IHtmlNode
  {

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

    IHtmlAttribute IHtmlElement.AddAttribute( string attributeName )
    {
      return Element.AddAttribute( attributeName );
    }

    #endregion

    #region IHtmlContainer 成员

    IEnumerable<IHtmlNode> IHtmlContainer.Nodes()
    {
      return Element.Nodes();
    }

    #endregion

    protected override sealed IHtmlNode Node
    {
      get { return Element; }
    }
  }
}
