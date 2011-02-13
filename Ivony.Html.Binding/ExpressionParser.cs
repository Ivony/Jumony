using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{
  /// <summary>
  /// 分析并计算样式表设置值里的表达式
  /// </summary>
  internal static class ExpressionParser
  {


    public static readonly Regex intergerRegex = new Regex( "^(\\-|\\+)?" + Regulars.integerPattern + "$", RegexOptions.Compiled );
    public static readonly Regex decimalRegex = new Regex( "^(\\-|\\+)?" + Regulars.decimalPattern + "$", RegexOptions.Compiled );


    public static readonly Regex quoteTextRegex = new Regex( "^" + Regulars.quoteTextPattern + "$", RegexOptions.Compiled );


    public static readonly Regex specialValueRegex = new Regex( @"^\<(?<name>\w+)\>$", RegexOptions.Compiled );
    public static readonly Regex environmentVariableRegex = new Regex( @"^\$(?<prefix>\w+):(?<name>\w+)$", RegexOptions.Compiled );


    public static readonly string listExpressionPattern = string.Format( @"\[((?<item>{0}|[^\\'"",]*)(,(?<item>{0}|[^\\'"",]*))*)?\]", Regulars.quoteTextPattern );
    public static readonly Regex listExpressionRegex = new Regex( "^" + listExpressionPattern + "$", RegexOptions.Compiled );


    public static object Evaluate( string expression )
    {
      expression = expression.Trim();

      //字符串表达式
      var quoteMatch = quoteTextRegex.Match( expression );
      if ( quoteMatch.Success )
        return Regulars.ResolveEscape( quoteMatch.Groups["quoteText"].Value );



      //数值表达式
      var intergerMatch = intergerRegex.Match( expression );
      var decimalMatch = decimalRegex.Match( expression );

      if ( intergerMatch.Success || decimalMatch.Success )
      {
        var d = decimal.Parse( expression );

        if ( intergerMatch.Success )
        {

          if ( d > int.MinValue && d < int.MaxValue )
            return (int) d;

          else if ( d > long.MinValue && d < long.MaxValue )
            return (long) d;
        }

        return d;
      }


      //列表表达式
      var listMatch = listExpressionRegex.Match( expression );
      if ( listMatch.Success )
        return listMatch.Groups["item"].Captures.Cast<Capture>().Select( c => Evaluate( c.Value ) ).ToArray();



      //特殊值表达式（例如null）
      var specialValueMatch = specialValueRegex.Match( expression );
      if ( specialValueMatch.Success )
      {
        switch ( specialValueMatch.Groups["name"].Value )
        {
          case "null":
            return null;
          default:
            throw new FormatException();
        }
      }


      //环境变量表达式
      var environmentVariableMatch = environmentVariableRegex.Match( expression );
      if ( environmentVariableMatch.Success )
      {
        string providerName = environmentVariableMatch.Groups["prefix"].Value;
        string exp =  environmentVariableMatch.Groups["name"].Value;

        return HtmlBinder.EvaluateExpression( providerName, exp );
      }

      return expression;
    }

  }
}
