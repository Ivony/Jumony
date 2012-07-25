using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Html;

namespace RegularsAssemblyBuilder
{
  class Program
  {
    static void Main( string[] args )
    {

      Regex.CompileToAssembly( GetRegexCompilationInfos(), new AssemblyName( "Ivony.Html.Regulars" ) );

    }


    public static RegexCompilationInfo[] GetRegexCompilationInfos()
    {
      return new[]
      {
        new RegexCompilationInfo( "^"+Ivony.Html.Regulars.attributeExpressionPattern+"$", RegexOptions.Compiled, "AttributeExpression", "Ivony.Html.Regulars", true ),
        new RegexCompilationInfo( "^"+Ivony.Html.Regulars.pseudoClassPattern+"$", RegexOptions.Compiled, "PseudoClass", "Ivony.Html.Regulars", true ),
        new RegexCompilationInfo( "^"+Ivony.Html.Regulars.elementExpressionPattern+"$", RegexOptions.Compiled, "ElementExpression", "Ivony.Html.Regulars", true ),
        new RegexCompilationInfo( "^"+Ivony.Html.Regulars.cssCasecadingSelectorPattern+"$", RegexOptions.Compiled, "CssCasecadingSelector", "Ivony.Html.Regulars", true ),
        new RegexCompilationInfo( "^"+Ivony.Html.Regulars.cssSelectorPattern+"$", RegexOptions.Compiled, "CssSelector", "Ivony.Html.Regulars", true ),
      };
    }


  }
}
