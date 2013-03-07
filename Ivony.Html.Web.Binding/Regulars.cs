using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public static class Regulars
  {

    public static readonly string ExpressionArgumentPattern = @"(?<args>(?<name>\w+)(=(?<value>[^\,]+))?)";
    public static readonly string BindingExpressionPattern = string.Format( @"(?<expression>\{Binding\s*({0}(\,\s*{0})*)?\})", ExpressionArgumentPattern );

  }
}
