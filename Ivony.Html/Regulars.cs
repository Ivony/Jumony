
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Ivony.Html
{
  /// <summary>
  /// 定义所有用于分析 HTML 和 CSS 选择器的正则表达式
  /// </summary>
  public static class Regulars
  {
    /// <summary>匹配用双引号标识的引用文本</summary>
    public const string dquoteTextPattern = @"(""(?<quoteText>(\\.|[^""\\])*)"")";
    /// <summary>匹配用单引号标识的引用文本</summary>
    public const string squoteTextPattern = @"('(?<quoteText>(\\.|[^'\\])*)')";
    /// <summary>匹配用引号标识的引用文本</summary>
    public const string quoteTextPattern = "(" + dquoteTextPattern + "|" + squoteTextPattern + ")";

    /// <summary>匹配CSS层叠关系选择符的正则表达式</summary>
    public const string relativeExpressionPattern = @"(?<relative>(\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";
    /// <summary>匹配CSS层叠关系选择符的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public const string relativeExpressionPatternNoGroup = @"((\p{Zs}+~\p{Zs}+)|(\p{Zs}+\+\p{Zs}+)|(\p{Zs}+\>\p{Zs}+)|\p{Zs}+)";

    /// <summary>匹配CSS属性选择器的正则表达式</summary>
    public static readonly string attributeExpressionPattern = string.Format( CultureInfo.InvariantCulture, @"\[(?<name>[\w-]+)((?<separator>(\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)(?<value>{0}|[^'""\]]*))?\]", quoteTextPattern );
    /// <summary>匹配CSS属性选择器的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public static readonly string attributeExpressionPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @"\[[\w-]+(((\|=)|(\*=)|(\~=)|(\$=)|(\!=)|(\^=)|=)({0}|[^'""\]]*))?\]", quoteTextPattern );

    /// <summary>匹配CSS伪类选择器的正则表达式</summary>
    public static readonly string pseudoClassPattern = string.Format( CultureInfo.InvariantCulture, @":(?<name>[\w-]+)(\((?<args>{0}|([^'""\(\)]+|(?<-open>\))|(?<open>\())*)(?(open)(?!))\))?", quoteTextPattern );
    /// <summary>匹配CSS伪类选择器的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public static readonly string pseudoClassPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @":([\w-]+)(\(({0}|([^'""\(\)]+|(?<-open>\))|(?<open>\())*)(?(open)(?!))\))?", quoteTextPattern );


    /// <summary>匹配CSS元素选择器的正则表达式</summary>
    public static readonly string elementExpressionPattern = string.Format( CultureInfo.InvariantCulture, @"(?<elementSelector>(?<name>\w+)?((#(?<identity>[\w-]+))|(\.(?<class>[\w-]+))+)?(?<attributeSelector>{0})*(?<pseudoClassSelector>{1})*)", attributeExpressionPatternNoGroup, pseudoClassPatternNoGroup );
    /// <summary>匹配CSS元素选择器的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public static readonly string elementExpressionPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @"((\w+)?((#([\w-]+))|(\.([\w-]+))+)?({0})*({1})*)", attributeExpressionPatternNoGroup, pseudoClassPatternNoGroup );

    /// <summary>匹配CSS层叠选择器的正则表达式</summary>
    public static readonly string cssCasecadingSelectorPattern = string.Format( CultureInfo.InvariantCulture, "(?<relativeSelector>(?<selector>{0})(?<relative>{1}))*(?<selector>{0})", elementExpressionPatternNoGroup, relativeExpressionPatternNoGroup );
    /// <summary>匹配CSS层叠选择器的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public static readonly string cssCasecadingSelectorPatternNoGroup = string.Format( CultureInfo.InvariantCulture, "({0}{1})*{0}", elementExpressionPatternNoGroup, relativeExpressionPatternNoGroup );

    /// <summary>匹配CSS选择器的正则表达式</summary>
    public static readonly string cssSelectorPattern = string.Format( CultureInfo.InvariantCulture, @"(?<selector>{0})(\s+,\s+(?<selector>{0}))*", cssCasecadingSelectorPatternNoGroup );
    /// <summary>匹配CSS选择器的正则表达式，这是不带分组名的版本，用于组合其他正则</summary>
    public static readonly string cssSelectorPatternNoGroup = string.Format( CultureInfo.InvariantCulture, @"({0})(\s+,\s+({0}))*", cssCasecadingSelectorPatternNoGroup );


    /// <summary>匹配任意十进制无符号整数</summary>
    public static readonly string integerPattern = "([1-9][0-9]*|0)";
    /// <summary>匹配任意十进制无符号小数</summary>
    public static readonly string decimalPattern = string.Format( CultureInfo.InvariantCulture, @"({0}(\.[0-9]*[1-9]))", integerPattern );


    /// <summary>匹配空白字符用于分割的正则表达式</summary>
    public static readonly Regex whiteSpaceSeparatorRegex = new Regex( @"\p{Zs}", RegexOptions.Compiled | RegexOptions.CultureInvariant );



    /// <summary>匹配 CSS 样式设置</summary>
    public static readonly string styleSettingPattern = string.Format( CultureInfo.InvariantCulture, @";?(?<name>[\w-]+)\s*:(?<value>({0}|[^'"";]+));", quoteTextPattern );

    /// <summary>匹配 CSS 样式规则</summary>
    public static readonly string styleRulePattern = string.Format( CultureInfo.InvariantCulture, @"(?<selector>{0})\s*{{(\s*(?<styleSetting>{1}))*\s*}}", cssSelectorPatternNoGroup, styleSettingPattern );
    /// <summary>匹配 CSS 样式表</summary>
    public static readonly string styleSheetPattern = string.Format( CultureInfo.InvariantCulture, @"^((\s*(?<styleRule>{0})\s*)+|\s*)$", styleRulePattern );

    /// <summary>
    /// 转换转义字符
    /// </summary>
    /// <param name="str">要执行转换的字符串</param>
    /// <returns>转换后的结果</returns>
    /// <remarks>此方法用于将字符串中的转义字符如"\t"替换为转义后的形式，例如"  "（跳格）。</remarks>
    public static string ResolveEscape( string str )
    {

      if ( str == null )
        return null;

      return str.Replace( "\\n", "\n" ).Replace( "\\r", "\r" ).Replace( "\\t", "\t" ).Replace( "\\\"", "\"" ).Replace( "\\\'", "\'" );
    }

    /// <summary>
    /// 查找捕获组在指定捕获内存在的捕获
    /// </summary>
    /// <param name="capture">要限定查找范围的捕获</param>
    /// <param name="group">要查找的捕获组</param>
    /// <returns>找到的捕获</returns>
    /// <remarks>这个扩展方法用于从一个大的捕获组的匹配中分离出子捕获组的匹配。</remarks>
    public static IEnumerable<Capture> FindCaptures( this Capture capture, Group group )
    {

      var start = capture.Index;
      var end = start + capture.Length;

      foreach ( Capture c in group.Captures )
      {
        if ( c.Index >= start && c.Index + c.Length <= end )
          yield return c;
      }
    }

  }
}
