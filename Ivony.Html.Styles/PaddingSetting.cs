using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  public static class PaddingSetting
  {

    public static CssStyle Padding( this CssStyle style, CssBox<CssLengthValue> value )
    {
      return style;
    }

    public static CssBox<CssLengthValue> Padding( this CssStyle style )
    {
      throw new NotImplementedException();
    }

  }
}
