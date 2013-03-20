using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{

  public static class DisplaySetting
  {

    public const string name = "display";

    public static DisplayValue Display( this StyleManager style )
    {
      throw new NotImplementedException();
    }

    public static StyleManager Display( this StyleManager style, DisplayValue value )
    {
      style.SetValue( name, value.ValueString );
      return style;
    }
  }


  public sealed class DisplayValue : EnumStyleValue
  {
    private DisplayValue( string value ) : base( value ) { }

    public static readonly DisplayValue Inline = new DisplayValue( "inline" );
    public static readonly DisplayValue Block = new DisplayValue( "block" );
    public static readonly DisplayValue None = new DisplayValue( "none" );
    public static readonly DisplayValue ListItem = new DisplayValue( "list-item" );
    public static readonly DisplayValue RunIn = new DisplayValue( "run-in" );
    public static readonly DisplayValue InlineBlock = new DisplayValue( "inline-block" );
    public static readonly DisplayValue Table = new DisplayValue( "table" );
    public static readonly DisplayValue InlineTable = new DisplayValue( "inline-table" );
    public static readonly DisplayValue TableRowGroup = new DisplayValue( "table-row-group" );
    public static readonly DisplayValue TableHeaderGroup = new DisplayValue( "table-header-group" );
    public static readonly DisplayValue TableFooterGroup = new DisplayValue( "table-footer-group" );
    public static readonly DisplayValue TableRow = new DisplayValue( "table-row" );
    public static readonly DisplayValue TableColumnGroup = new DisplayValue( "table-column-group" );
    public static readonly DisplayValue TableColumn = new DisplayValue( "table-column" );
    public static readonly DisplayValue TableCell = new DisplayValue( "table-cell" );
    public static readonly DisplayValue TableCaption = new DisplayValue( "table-caption" );
  }
}
