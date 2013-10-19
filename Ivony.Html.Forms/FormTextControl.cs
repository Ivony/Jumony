using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 文本控件的基类
  /// </summary>
  public abstract class FormTextControl : FormControl
  {

    protected FormTextControl( HtmlForm form, IHtmlElement element )
      : base( form )
    {
      Element = element;
    }

    public IHtmlElement Element
    {
      get;
      private set;
    }


    public override string Name
    {
      get { return Element.Attribute( "name" ).Value(); }
    }

    public abstract string Value
    {
      get { return Element.Attribute( "value" ).Value(); }
    }

    public override bool CanSetValue( string value )
    {
      return true;
    }
  }
}
