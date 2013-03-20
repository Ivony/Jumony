using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  public static class VisibilitySetting
  {

    public const string name = "visibility";

    public static VisibilityValue Visibility( this StyleManager style )
    {
      throw new NotImplementedException();
    }

    public static StyleManager Visibility( this StyleManager style, VisibilityValue value )
    {
      style.SetValue( name, value.ValueString );
      return style;
    }
  }



  public sealed class VisibilityValue : EnumStyleValue
  {

    private VisibilityValue( string value ) : base( value ) { }

    public static readonly VisibilityValue Visible = new VisibilityValue( "visible" );
    public static readonly VisibilityValue Hidden = new VisibilityValue( "hidden" );
    public static readonly VisibilityValue Collapse = new VisibilityValue( "collapse" );

  }
}
