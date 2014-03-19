using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class HtmlInputText : FormTextControl
  {

    internal HtmlInputText( HtmlForm form, IHtmlElement element )
      : base( form, element )
    {
      var value = element.Attribute( "value" ).Value() ?? "";

    }



    protected override string GetValue()
    {
      return Element.Attribute( "value" ).Value() ?? "";
    }


    protected override void SetValue( string value )
    {
      value = value.Replace( "\r", "" ).Replace( "\n", "" );
      Element.SetAttribute( "value", value );
    }

    protected override bool CanSetValue( string value, out string message )
    {
      if ( !base.CanSetValue( value, out message ) )
        return false;


      if ( !Form.Configuration.IgnoreNewlineInTextbox && (value.Contains( "\r" ) || value.Contains( "\n" )) )
      {
        message = "单行文本框不能输入多行文本值";
        return false;
      }

      else
        return true;
    }
  }
}
