using Ivony.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{




  /// <summary>
  /// 定义绑定表达式，绑定表达式可以由属性值或者元素来定义。
  /// </summary>
  public abstract class BindingExpression : IBindingExpressionValueObject
  {


    /// <summary>
    /// 表达式名称
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 派生类实现此属性提供参数值列表
    /// </summary>
    public abstract BindingExpressionArgumentCollection Arguments { get; }


    public bool TryGetValue<T>( IBindingExpressionEvaluator evaluator, string name, out T value )
    {
      return Arguments.TryGetValue( evaluator, name, out value );
    }


    public T GetValue<T>( IBindingExpressionEvaluator evaluator, string name )
    {
      return Arguments.GetValue<T>( evaluator, name );
    }



    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="evaluator">用于解析绑定表达式并计算绑定值的计算器</param>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( string expression )
    {
      return ParseExpression( expression, 0 );
    }

    /// <summary>
    /// 解析绑定表达式
    /// </summary>
    /// <param name="evaluator">用于解析绑定表达式并计算绑定值的计算器</param>
    /// <param name="expression">要从中解析的绑定表达式的字符串</param>
    /// <param name="index">解析绑定表达式的开始位置</param>
    /// <returns>解析后的结果</returns>
    public static BindingExpression ParseExpression( string expression, int index )
    {

      if ( string.IsNullOrEmpty( expression ) )
        return null;

      if ( expression[index] != '{' )
        return null;

      if ( tokenizer == null )
        tokenizer = new BindingExpressionTokenizer();

      return tokenizer.Parse( expression, index );

    }


    [ThreadStatic]
    private static BindingExpressionTokenizer tokenizer;//设置为线程内单例，提高利用率。




    /// <summary>
    /// 绑定表达式解析器
    /// </summary>
    private class BindingExpressionTokenizer : TokenizerBase
    {




      private static readonly Regex EName = new Regex( @"\G[a-zA-z_][a-zA-Z_0-9-]*", RegexOptions.Compiled | RegexOptions.CultureInvariant );
      private static readonly Regex ArgumentValue = new Regex( @"\G([^{},]|\{\{|\}\}|,,)+", RegexOptions.Compiled | RegexOptions.CultureInvariant );


      /// <summary>
      /// 解析绑定表达式为 BindingExpression 对象
      /// </summary>
      /// <param name="evaluator">用于计算绑定表达式值的计算器</param>
      /// <param name="text">要分析的文本</param>
      /// <param name="index">开始分析的位置</param>
      /// <returns>解析出的 BindingExpression 对象</returns>
      public BindingExpression Parse( string text, int index )
      {
        lock ( SyncRoot )
        {

          Initialize( text, index );

          var expression = Parse();
          if ( expression == null )
            return null;

          if ( Scaner.IsEnd )
            return expression;

          return null;
        }
      }

      private BindingExpression Parse()
      {
        if ( !Match( '{' ).HasValue )
          return null;

        var match = Match( EName );
        if ( match == null )
          return null;


        var expression = new ParsedBindingExpression( match.Value );

        if ( Match( '}' ).HasValue )
          return expression;//解析成功


        match = Match( WhiteSpace );
        if ( match == null || match.Value.Length == 0 )
          return null;

        while ( true )
        {
          match = Match( EName );
          if ( match == null )
            return null;

          ParseValue( match.Value, expression.Arguments );


          if ( Match( ',' ).HasValue )
          {
            Match( WhiteSpace );
            continue;
          }

          else if ( Match( '}' ).HasValue )//解析成功
          {
            expression.Arguments.SetCompleted();
            return expression;
          }


          else
            return null;                   //解析失败
        }


      }

      private void ParseValue( string argumentName, BindingExpressionArgumentCollection arguments )
      {
        if ( Match( '=' ) == null )
          arguments.Add( argumentName );



        Match match = Match( ArgumentValue );
        if ( match != null )
          arguments.Add( argumentName, match.Value.Replace( "{{", "{" ).Replace( "}}", "}" ).Replace( ",,", "," ) );

        else if ( IsMatch( '{' ) )
          arguments.Add( argumentName, Parse() );

        else
          arguments.Add( argumentName, "" );

      }
    }



    private class ParsedBindingExpression : BindingExpression
    {

      private string _name;
      private BindingExpressionArgumentCollection _arguments = new BindingExpressionArgumentCollection();


      public ParsedBindingExpression( string expressionName ) { _name = expressionName; }



      public override string Name
      {
        get { return _name; }
      }

      public override BindingExpressionArgumentCollection Arguments
      {
        get { return _arguments; }
      }

    }

    object IBindingExpressionValueObject.GetValue( IBindingExpressionEvaluator evaluator )
    {
      return evaluator.GetValue( this );
    }
  }
}
