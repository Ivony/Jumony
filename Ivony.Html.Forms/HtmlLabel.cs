using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  /// <summary>
  /// 表示 label 元素
  /// </summary>
  public class HtmlLabel : IHtmlFormElement
  {

    private readonly HtmlForm _form;
    private readonly IHtmlElement _element;
    private readonly IHtmlElement _bindElement;


    public HtmlLabel( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.Equals( "label", StringComparison.InvariantCultureIgnoreCase ) )
        throw new NotSupportedException( "不能将非 label 元素转换为 HtmlLabel 对象" );

      _form = form;
      _element = element;

      var forId = Element.Attribute( "for" ).Value();
      if ( forId != null )
      {
        _bindElement = Form.Element.Find( "#" + forId ).SingleOrDefault();
      }
    }

    public HtmlForm Form
    {
      get { return _form; }
    }

    public IHtmlElement Element
    {
      get { return _element; }
    }

    public IHtmlElement BindElement
    {
      get { return _bindElement; }
    }

    public string Text
    {
      get { return Element.InnerText(); }
    }
  }
}
