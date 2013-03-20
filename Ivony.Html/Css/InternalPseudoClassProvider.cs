using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;

namespace Ivony.Html
{

  /// <summary>
  /// 内置伪类选择器
  /// </summary>
  internal class InternalPseudoClassProvider : ICssPseudoClassProvider
  {



    public ICssPseudoClassSelector CreateSelector( string name, string args )
    {
      if ( name == null )
        throw new ArgumentNullException( "name" );



      name = name.ToLowerInvariant();

      switch ( name )
      {
        case "nth-child":
        case "nth-last-child":
        case "nth-of-type":
        case "nth-last-of-type":
        case "first-child":
        case "last-child":
        case "first-of-type":
        case "last-of-type":
          return new NthPseudoClass( name, args );

        case "only-child":
        case "only-of-type":
        case "empty":
          if ( args != null )
            throw new FormatException( string.Format( CultureInfo.InvariantCulture, "{0} 伪类不能有参数", name ) );
          return new CountPseudoClass( name );

        default:
          throw new NotSupportedException();
      }

    }

    public static void RegisterInternalPseudoClasses()
    {

      var provider = new InternalPseudoClassProvider();

      CssParser.RegisterPseudoClassProvider( "nth-child", provider );
      CssParser.RegisterPseudoClassProvider( "nth-last-child", provider );
      CssParser.RegisterPseudoClassProvider( "nth-of-type", provider );
      CssParser.RegisterPseudoClassProvider( "nth-last-of-type", provider );
      CssParser.RegisterPseudoClassProvider( "first-child", provider );
      CssParser.RegisterPseudoClassProvider( "last-child", provider );
      CssParser.RegisterPseudoClassProvider( "first-of-type", provider );
      CssParser.RegisterPseudoClassProvider( "last-of-type", provider );
      CssParser.RegisterPseudoClassProvider( "only-child", provider );
      CssParser.RegisterPseudoClassProvider( "only-of-type", provider );
      CssParser.RegisterPseudoClassProvider( "empty", provider );

    }



    private class NthPseudoClass : ICssPseudoClassSelector
    {

      /// <summary>匹配任意十进制无符号整数</summary>
      private static readonly string integerPattern = "([1-9][0-9]*|0)";

      private static readonly string expressionPattern = @"(?<augend>#interger)|((?<multiplier>((\+|\-)?#interger)|\-)n\p{Zs}*(?<augend>(\+|\-)\p{Zs}*#interger)?)".Replace( "#interger", integerPattern );
      private static readonly Regex expressionRegex = new Regex( "^(" + expressionPattern + ")$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

      private string _name;
      private string _args;

      private int multiplier;
      private int augend;


      private bool ofType;
      private bool last;
      private bool nth;


      public NthPseudoClass( string name, string args )
      {

        _name = name.ToLowerInvariant();

        var correctNames = new[] { "nth-child", "nth-last-child", "nth-of-type", "nth-last-of-type", "first-child", "last-child", "first-of-type", "last-of-type" };

        if ( !correctNames.Contains( _name ) )
          throw new InvalidOperationException();


        nth = _name.StartsWith( "nth-" );
        last = _name.Contains( "last-" );
        ofType = _name.Contains( "-of-type" );



        _args = null;

        if ( args != null )
          _args = args.Trim().ToLowerInvariant();

        if ( !nth )//没有 nth 前缀说明获取第一个
        {
          if ( !string.IsNullOrEmpty( args ) )//没有nth前缀的不能有参数
            throw new FormatException();

          _args = "1";
        }



        if ( _args == "odd" )
          _args = "2n+1";

        if ( args == "even" )
          _args = "2n";


        var match = expressionRegex.Match( _args );

        if ( !match.Success )
          throw new FormatException();


        multiplier = 0;//默认值是0，表示没有倍数选择
        augend = 0;

        if ( match.Groups["multiplier"].Success )
        {
          string _multiplier = match.Groups["multiplier"].Value;
          if ( _multiplier == "-" )//如果只有一个负号
            multiplier = -1;//那意味着负1
          else
            multiplier = int.Parse( match.Groups["multiplier"].Value, CultureInfo.InvariantCulture );
        }

        if ( match.Groups["augend"].Success )
          augend = int.Parse( Regex.Replace( match.Groups["augend"].Value, @"\p{Zs}", "" ), CultureInfo.InvariantCulture );//这里的正则用于去掉符号与数字之间的空白
      }


      bool ICssPseudoClassSelector.IsEligible( IHtmlElement element )
      {

        if ( element == null )
          throw new ArgumentNullException( "element" );


        List<IHtmlElement> siblings;

        if ( ofType )
          siblings = element.Siblings( element.Name ).ToList();
        else
          siblings = element.Siblings().ToList();

        if ( last )
          siblings.Reverse();

        return Check( siblings.IndexOf( element ) );
      }


      /// <summary>
      /// 检查元素所处的索引位置是否符合参数表达式要求。
      /// </summary>
      /// <param name="index">所处的索引位置</param>
      /// <returns></returns>
      public bool Check( int index )
      {
        index += 1;
        index = index - augend;//计算从 augend 开始的偏移量

        if ( multiplier == 0 )//如果没有倍数选择，那么判断元素是否恰好在 augend 的位置。
          return index == 0;

        if ( multiplier > 0 )//如果倍数大于0
        {
          if ( index < 0 )//在 augend 之前的元素被忽略
            return false;

          if ( index % multiplier != 0 )//看位置是否符合倍数
            return false;

          return true;
        }


        if ( multiplier < 0 )//如果倍数小于0
        {
          index = -index;//反转索引位置，换算成从 augend 往前的偏移量

          if ( index < 0 )//在 augend 之后的元素忽略
            return false;

          if ( index % Math.Abs( multiplier ) != 0 )//看位置是否符合倍数
            return false;

          return true;
        }


        throw new FormatException( "分析nth伪类时出现了一个其他未知情况" );

      }


      public override string ToString()
      {

        string argsExp = null;
        if ( multiplier != 0 )
          argsExp = multiplier + "n";

        if ( argsExp != null )
        {
          if ( augend < 0 )
            argsExp += "-";
          else
            argsExp += "+";

          argsExp += Math.Abs( augend ).ToString( CultureInfo.InvariantCulture );
        }
        else
          argsExp = augend.ToString( CultureInfo.InvariantCulture );

        return string.Format( CultureInfo.InvariantCulture, ":nth-child({0})", argsExp );
      }

    }



    private class CountPseudoClass : ICssPseudoClassSelector
    {


      private readonly string _name;


      private static readonly string[] correctNames = new[] { "only-child", "only-of-type", "empty" };



      public CountPseudoClass( string name )
      {
        _name = name.ToLowerInvariant();


        if ( !correctNames.Contains( _name ) )
          throw new InvalidOperationException();

      }


      bool ICssPseudoClassSelector.IsEligible( IHtmlElement element )
      {

        if ( element == null )
          throw new ArgumentNullException( "element" );


        switch ( _name )
        {
          case "only-child":
            return element.Siblings().Count() == 1;
          case "only-of-type":
            return element.Siblings( element.Name ).Count() == 1;
          case "empty":
            return element.Nodes().Count( n => n is IHtmlElement || n is IHtmlTextNode ) == 0;//只有元素和文本节点视为有内容 http://www.w3.org/TR/2011/REC-css3-selectors-20110929/#empty-pseudo

          default:
            throw new InvalidOperationException();
        }
      }



      public override string ToString()
      {
        return ":" + _name;
      }

    }



  }
}
