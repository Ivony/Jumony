using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  public class HtmlTextArea : IHtmlTextControl
  {


    private readonly HtmlForm _form;
    private readonly IHtmlElement _element;



    public HtmlTextArea( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "textarea" ) )
        throw new InvalidOperationException( "只有 <textarea> 元素才能转换为 HtmlTextArea 对象" );

      _form = form;
      _element = element;
    }


    public IHtmlElement Element
    {
      get { return _element; }
    }


    public string TextValue
    {
      get { return _element.InnerText(); }
      set { _element.InnerText( value ); }
    }

    public string Name
    {
      get { return _element.Attribute( "name" ).AttributeValue; }
    }

    public HtmlForm Form
    {
      get { return _form; }
    }


    #region IHtmlFocusableControl 成员

    string IHtmlFocusableControl.ElementId
    {
      get { return Element.Identity(); }
    }

    #endregion
  }
}
