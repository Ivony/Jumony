using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  public static class PaddingSetting
  {

    public static StyleManager Padding( this StyleManager style, CssBox<CssLengthValue> value )
    {
      return style;
    }

    public static CssBox<CssLengthValue> Padding( this StyleManager style )
    {
      throw new NotImplementedException();
    }

  }
}
