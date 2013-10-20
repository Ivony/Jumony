using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  public class HtmlTextArea : FormTextControl
  {

    internal HtmlTextArea( HtmlForm form, IHtmlElement element )
      : base( form, element )
    {
    }



    protected override string GetValue()
    {
      return Element.InnerText();
    }

    protected override void SetValue( string value )
    {
      Element.InnerText( value );
    }


  }
}
