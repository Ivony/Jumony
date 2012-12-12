using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Ivony.Html.Forms.Validation
{
  public static class ValidationRegulars
  {
    public static readonly string domainNamePattern = @"[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+";
    public static readonly string emailPattern = string.Format( CultureInfo.InvariantCulture, @"\w+@({0})", domainNamePattern );

  }
}
