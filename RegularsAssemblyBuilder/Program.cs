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

      Regex.CompileToAssembly( GetRegexCompilationInfos(), new AssemblyName( "Ivony.Html.Parser.Regulars" ) );

    }


    public static RegexCompilationInfo[] GetRegexCompilationInfos()
    {
      return new[]
      {

        new RegexCompilationInfo( "^"+Regulars.tagNamePattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "TagName", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( "^"+Regulars.attributeNamePattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant |  RegexOptions.Singleline |RegexOptions.ExplicitCapture, "AttributeName", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( Regulars.attributePattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "Attribute", "Ivony.Html.Parser.Regulars", true ),

        new RegexCompilationInfo( @"\G"+Regulars.beginTagPattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant |  RegexOptions.Singleline |RegexOptions.ExplicitCapture, "BeginTag", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( @"\G"+Regulars.endTagPattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "EndTag", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( @"\G"+Regulars.doctypeDeclarationPattern+"$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "DoctypeDeclaration", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( @"\G"+Regulars.commentPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "CommentTag", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( @"\G"+Regulars.specialTagPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant |  RegexOptions.Singleline |RegexOptions.ExplicitCapture, "SpecialTag", "Ivony.Html.Parser.Regulars", true ),
        new RegexCompilationInfo( @"\G(?<tag>\<.+?\>)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture, "HtmlTag", "Ivony.Html.Parser.Regulars", true ),
      };
    }


  }
}
