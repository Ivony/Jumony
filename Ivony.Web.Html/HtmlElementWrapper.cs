using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public abstract class HtmlElementWrapper : IHtmlElement
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

    void IHtmlElement.BindCore( HtmlBindingContext context, string path, string value, BindingNullBehavior nullBehavior )
    {
      Element.BindCore( context, path, value, nullBehavior );
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

    #region IHtmlNode 成员

    IHtmlContainer IHtmlNode.Parent
    {
      get { return Element.Parent; }
    }

    object IHtmlNode.NodeObject
    {
      get { return Element.NodeObject; }
    }

    void IHtmlNode.Remove()
    {
      Element.Remove();
    }

    IHtmlDocument IHtmlNode.Document
    {
      get { return Element.Document; }
    }

    #endregion
  }
}
