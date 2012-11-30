using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace RegularsAssemblyBuilder
{
  class Program
  {
    static void Main( string[] args )
    {

      Regex.CompileToAssembly( GetRegexCompilationInfos(), new AssemblyName( "Ivony.Html.Parser.HtmlRegulars" ) );

    }


    public static RegexCompilationInfo[] GetRegexCompilationInfos()
    {
      return new[]
      {
        new RegexCompilationInfo( "^"+Regulars.beginTagPattern+"$", RegexOptions.Compiled, "BeginTag", "Ivony.Html.Parser.HtmlRegulars", true ),
        new RegexCompilationInfo( "^"+Regulars.endTagPattern+"$", RegexOptions.Compiled, "EndTag", "Ivony.Html.Parser.HtmlRegulars", true ),
        new RegexCompilationInfo( "^"+Regulars.commentPattern+"$", RegexOptions.Compiled, "CommentTag", "Ivony.Html.Parser.HtmlRegulars", true ),
        new RegexCompilationInfo( "^"+Regulars.specialTagPattern+"$", RegexOptions.Compiled, "SpecialTag", "Ivony.Html.Parser.HtmlRegulars", true ),
        new RegexCompilationInfo( Regulars.tagPattern, RegexOptions.Compiled, "HtmlTag", "Ivony.Html.Parser.HtmlRegulars", true ),
      };
    }


  }
}
