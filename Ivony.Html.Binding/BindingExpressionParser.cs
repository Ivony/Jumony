using Ivony.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Binding
{
  public abstract partial class BindingExpression
  {

    /// <summary>
    /// 绑定表达式解析器
    /// </summary>
    private class BindingExpressionParser : TokenizerBase
    {




      private static readonly Regex EName = new Regex( @"\G[a-zA-z_][a-zA-Z_0-9-]*", RegexOptions.Compiled | RegexOptions.CultureInvariant );
      private static readonly Regex ArgumentValue = new Regex( @"\G([^{},]|\{\{|\}\}|,,)+", RegexOptions.Compiled | RegexOptions.CultureInvariant );


      /// <summary>
      /// 解析绑定表达式为 BindingExpression 对象
      /// </summary>
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

        var name = match.Value;

        if ( Match( '}' ).HasValue )
          return new ParsedBindingExpression( name );//解析成功


        match = Match( WhiteSpace );
        if ( match == null || match.Value.Length == 0 )
          return null;


        var arguments = new Dictionary<string, IBindingExpressionValueObject>( StringComparer.OrdinalIgnoreCase );

        while ( true )
        {
          match = Match( EName );
          if ( match == null )
            return null;               //未能解析参数名，解析失败



          arguments.Add( match.Value, ParseValue() );

          if ( Match( '}' ).HasValue ) //解析成功
            return new ParsedBindingExpression( name, arguments );


          else if ( !Match( ',' ).HasValue )
            return null;               //未能解析出参数分隔符，解析失败


          Match( WhiteSpace );
        }
      }



      private IBindingExpressionValueObject ParseValue()
      {
        if ( Match( '=' ) == null )
          return new LiteralValue( null );


        Match match = Match( ArgumentValue );
        if ( match != null )
          return new LiteralValue( match.Value.Replace( "{{", "{" ).Replace( "}}", "}" ).Replace( ",,", "," ) );

        else if ( IsMatch( '{' ) )
          return Parse();

        else
          return new LiteralValue( "" );

      }
    }

  }
}
