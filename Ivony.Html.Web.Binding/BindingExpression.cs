using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Web.Binding
{

  public class BindingExpression
  {


    private static readonly Regex bindingExpressionRegex = new Regex( "^" + Regulars.BindingExpressionPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled );



    public static IDictionary<string, string> ParseExpression( IHtmlAttribute attribute )
    {
      var expression = attribute.Value();

      if ( expression == null )
        return null;

      var match = bindingExpressionRegex.Match( expression );
      if ( match == null || !match.Success )
        return null;

      return ParseExpression( match );
    }



    private static IDictionary<string, string> ParseExpression( Match match )
    {
      var args = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["args"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).FirstOrDefault().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).FirstOrDefault().Value;

        args[name] = value;
      }

      return args;

    }
  }
}
