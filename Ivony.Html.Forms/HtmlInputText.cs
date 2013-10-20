using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class HtmlInputText : FormTextControl
  {

    internal HtmlInputText( HtmlForm form, IHtmlElement element ) : base( form, element ) { }



    protected override string GetValue()
    {
      return Element.Attribute( "value" ).Value() ?? "";
    }

    protected override void SetValue( string value )
    {
      var singleline = value.Replace( "\r", "" ).Replace( "\n", "" );
      if ( singleline.Length != value.Length && !Form.Configuration.IgnoreNewline )
        throw new FormValueFormatException( this, "单行文本框不能输入多行文本值" );

      Element.SetAttribute( "value", value );
    }
  }
}
