using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  /// <summary>
  /// 表示 label 元素
  /// </summary>
  public class HtmlLabel : IHtmlFormElement
  {

    private readonly HtmlForm _form;
    private readonly IHtmlElement _element;
    private readonly string _forId;


    public HtmlLabel( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "label" ) )
        throw new NotSupportedException( "不能将非 label 元素转换为 HtmlLabel 对象" );

      _form = form;
      _element = element;

      _forId = Element.Attribute( "for" ).Value();
    }

    public HtmlForm Form
    {
      get { return _form; }
    }

    public IHtmlElement Element
    {
      get { return _element; }
    }

    public IHtmlElement ForElement
    {
      get
      {
        if ( string.IsNullOrEmpty( ForElementId ) )
          return null;

        return Form.Element.Find( "#" + ForElementId ).SingleOrDefault();
      }
    }

    public string ForElementId
    {
      get { return _forId; }
    }

    public string Text
    {
      get { return Element.InnerText(); }
    }
  }
}
