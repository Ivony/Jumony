
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
namespace Ivony.Html
{
  public static class Regulars
  {
    public const string dquoteTextPattern = @"(""(?<quoteText>(\\.|[^""\\])*)"")";
    public const string squoteTextPattern = @"('(?<quoteText>(\\.|[^'\\])*)')";
    public const string quoteTextPattern = "(" + dquoteTextPattern + "|" + squoteTextPattern + ")";

    public const string relativeExpressionPattern = @"(?<relative>(\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";
    public const string relativeExpressionPatternNoGroup = @"((\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";

    public static readonly string attributeExpressionPattern = string.Format( CultureInfo.InvariantCulture, @"\[(?<name>\w+)((?<separator>(\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)(?<value>{0}|[^'""\]]*))?\]", quoteTextPattern );
    public static readonly string attributeExpressionPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @"\[\w+(((\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)({0}|[^'""\]]*))?\]", quoteTextPattern );

    public static readonly string pseudoClassPattern = string.Format( CultureInfo.InvariantCulture, @":(?<name>[\w-]+)(\((?<args>{0}|[^'""\)]*)\))?", quoteTextPattern );
    public static readonly string pseudoClassPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @":([\w-]+)(\(({0}|[^'""\)]*)\))?", quoteTextPattern );

    public static readonly string elementExpressionPattern = string.Format( CultureInfo.InvariantCulture, @"(?<elementSelector>(?<name>\w+)?((#(?<identity>\w+))|(\.(?<class>\w+)))?(?<attributeSelector>{0})*(?<pseudoClassSelector>{1})*)", attributeExpressionPatternNoGroup, pseudoClassPatternNoGroup );
    public static readonly string elementExpressionPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @"((\w+)?((#(\w+))|(\.(\w+)))?({0})*({1})*)", attributeExpressionPatternNoGroup, pseudoClassPatternNoGroup );

    public static readonly string extraExpressionPattern = string.Format( CultureInfo.InvariantCulture, "{0}{1}", relativeExpressionPattern, elementExpressionPattern );
    public static readonly string extraExpressionPatternNoGroup = string.Format( CultureInfo.InvariantCulture, "(?<extra>{0}{1})", relativeExpressionPatternNoGroup, elementExpressionPatternNoGroup );

    public static readonly string cssSelectorPattern = string.Format( CultureInfo.InvariantCulture, "{0}{1}*", elementExpressionPattern, extraExpressionPatternNoGroup );
    public static readonly string cssSelectorPatternNoGroup = string.Format( CultureInfo.InvariantCulture, "{0}{1}*", elementExpressionPatternNoGroup, extraExpressionPatternNoGroup );

    public static readonly string integerPattern = "([1-9][0-9]*|0)";
    public static readonly string decimalPattern = string.Format( CultureInfo.InvariantCulture, @"({0}(\.[0-9]*[1-9]))", integerPattern );


    public static readonly string styleSettingPattern = string.Format( CultureInfo.InvariantCulture, @"\s*(?<name>[\w-]+)\s*:(?<value>({0}|[^'"";])+);\s*", quoteTextPattern );

    public static readonly string styleRulePattern = string.Format( CultureInfo.InvariantCulture, @"(?<selector>{0})\s*{{(?<styleSetting>{1})*}}", cssSelectorPatternNoGroup, styleSettingPattern );
    public static readonly string styleSheetPattern = string.Format( CultureInfo.InvariantCulture, @"^((\s*(?<styleRule>{0})\s*)+|\s*)$", styleRulePattern );


    public static string ReplaceEscape( string str )
    {
      return str.Replace( "\\n", "\n" ).Replace( "\\r", "\r" ).Replace( "\\t", "\t" ).Replace( "\\\"", "\"" ).Replace( "\\\'", "\'" );
    }


    public static IEnumerable<Capture> FindCaptures( this Capture capture, Group group )
    {
      foreach ( Capture c in group.Captures )
      {
        if ( c.Index >= capture.Index && c.Index + c.Length <= capture.Index + capture.Length )
          yield return c;
      }
    }

  }
}
