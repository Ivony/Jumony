using Ivony.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{




  /// <summary>
  /// 定义绑定表达式，绑定表达式可以由属性值或者元素来定义。
  /// </summary>
  public abstract class BindingExpression
  {


    /// <summary>
    /// 表达式名称
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 参数值
    /// </summary>
    public abstract IDictionary<string, string> Arguments { get; }




    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="evaluator">用于解析绑定表达式并计算绑定值的计算器</param>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( IBindingExpressionEvaluator evaluator, string expression )
    {
      return ParseExpression( evaluator, expression, 0 );
    }

    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="evaluator">用于解析绑定表达式并计算绑定值的计算器</param>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <param name="index">解析绑定表达式的开始位置</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( IBindingExpressionEvaluator evaluator, string expression, int index )
    {

      if ( string.IsNullOrEmpty( expression ) )
        return null;

      if ( expression[index] != '{' )
        return null;

      if ( tokenizer == null )
        tokenizer = new BindingExpressionTokenizer();

      return tokenizer.Parse( evaluator, expression, index );

    }


    [ThreadStatic]
    private static BindingExpressionTokenizer tokenizer;//设置为线程内单例，提高利用率。




    /// <summary>
    /// 绑定表达式解析器
    /// </summary>
    private class BindingExpressionTokenizer : TokenizerBase
    {




      private static readonly Regex EName = new Regex( @"\G[a-zA-z_][a-zA-Z_0-9-]*", RegexOptions.Compiled | RegexOptions.CultureInvariant );


      /// <summary>
      /// 解析绑定表达式为 BindingExpression 对象
      /// </summary>
      /// <param name="evaluator">用于计算绑定表达式值的计算器</param>
      /// <param name="text">要分析的文本</param>
      /// <param name="index">开始分析的位置</param>
      /// <returns>解析出的 BindingExpression 对象</returns>
      public BindingExpression Parse( IBindingExpressionEvaluator evaluator, string text, int index )
      {
        lock ( SyncRoot )
        {

          Initialize( text, index );

          var expression = Parse( evaluator );
          if ( expression == null )
            return null;

          if ( Scaner.IsEnd )
            return expression;

          return null;
        }
      }

      private BindingExpression Parse( IBindingExpressionEvaluator evaluator )
      {
        if ( !Match( '{' ).HasValue )
          return null;

        var match = Match( EName );
        if ( match == null )
          return null;

        var name = match.Value;


        if ( Match( '}' ).HasValue )
          return new ParsedBindingExpression( name );//解析成功


        match = Match( WhiteSpace );
        if ( match == null || match.Value.Length == 0 )
          return null;

        Dictionary<string, string> args = new Dictionary<string, string>();

        while ( true )
        {

          string argName, argValue;

          match = Match( EName );
          if ( match == null )
            return null;

          argName = match.Value;

          if ( Match( '=' ).HasValue )
          {

            match = Match( ArgumentValue );
            if ( match == null )
            {

              argValue = "";

              if ( IsMatch( '{' ) )
              {
                var expression = Parse( evaluator );
                argValue = evaluator.GetValue( expression );
              }


            }

            else
              argValue = match.Value.Replace( "{{", "{" ).Replace( "}}", "}" ).Replace( ",,", "," );
          }

          else
            argValue = null;


          args.Add( argName, argValue );


          if ( Match( '}' ).HasValue )
            return new ParsedBindingExpression( name, args );//解析成功


          if ( Match( ',' ).HasValue )
          {
            Match( WhiteSpace );
            continue;
          }

          return null;
        }
      }


      Regex ArgumentValue = new Regex( @"\G([^{},]|\{\{|\}\}|,,)+", RegexOptions.Compiled | RegexOptions.CultureInvariant );
    }



    private class ParsedBindingExpression : BindingExpression
    {

      private string _name;
      private Dictionary<string, string> _arguments;


      public ParsedBindingExpression( string expressionName ) : this( expressionName, new Dictionary<string, string>() ) { }

      public ParsedBindingExpression( string expressionName, Dictionary<string, string> arguments )
      {
        _name = expressionName;
        _arguments = arguments;
      }


      public override string Name
      {
        get { return _name; }
      }

      public override IDictionary<string, string> Arguments
      {
        get { return new Dictionary<string, string>( _arguments ); }
      }
    }


  }
}
