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
      MaxLength = GetMaxLength();
    }


    public int? MaxLength
    {
      get;
      private set;
    }


    private int? GetMaxLength()
    {
      int value;
      if ( int.TryParse( Element.Attribute( "maxlength" ).Value(), out value ) )
        return value;


      if ( Form.Configuration.ExceptionOnAttributeError )
        throw new FormControlException( this, "maxlength 属性设置错误" );

      Element.RemoveAttribute( "maxlength" );
      return null;
    }

  }
}
