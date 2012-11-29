using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
  public static class CssParser
  {

    /// <summary>
    /// 从选择器表达式创建选择器对象
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    public static ICssSelector ParseSelector( string expression )
    {

      var selector = selectorsCache[expression] as ICssSelector;
      if ( selector != null )
        return selector;

      var enumerator = new CharEnumerator( expression );

      selector = ParseSelector( enumerator );

      selectorsCache[expression] = selector;

      return selector;
    }

    private static Hashtable selectorsCache = new Hashtable();




    /// <summary>
    /// 从选择器表达式创建元素选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    public static CssElementSelector ParseElementSelector( string expression )
    {
      var enumerator = new CharEnumerator( expression );

      return ParseElementSelector( enumerator );
    }



    /// <summary>
    /// 创建层级选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定，上溯时不超出此范畴</param>
    /// <returns>带范畴限定的层叠选择器</returns>
    public static ICssSelector Create( IHtmlContainer scope, string expression )
    {
      return CssCasecadingSelector.Create( new CssAncetorRelativeSelector( new ContainerRestrict( scope ) ), CssParser.ParseSelector( expression ) );
    }



    /// <summary>
    /// 解析并创建选择器对象
    /// </summary>
    /// <param name="enumerator">用于读取选择器表达式的枚举器</param>
    /// <returns>选择器对象</returns>
    private static ICssSelector ParseSelector( CharEnumerator enumerator )
    {


      var selectorList = new List<ICssSelector>();

      ICssSelector selector = null;

      selector = ParseElementSelector( enumerator );

      while ( true )
      {
        SkipWhiteSpace( enumerator );

        if ( enumerator.Current == char.MinValue )
        {
          if ( selectorList.Any() )
          {
            if ( selector == null )
              throw FormatError( enumerator );

            selectorList.Add( selector );
            return new CssMultipleSelector( selectorList.ToArray() );
          }

          else
            return selector;
        }

        var ch = enumerator.Current;
        if ( ch == '>' || ch == '+' || ch == '~' )
        {
          EnsureNext( enumerator );
          SkipWhiteSpace( enumerator );

          var right = ParseElementSelector( enumerator );
          if ( right == null )
            throw FormatError( enumerator );

          selector = CreateCasecadingSelector( selector, ch, right );
        }
        else if ( ch == ',' )
        {
          EnsureNext( enumerator );
          SkipWhiteSpace( enumerator );

          selectorList.Add( selector );
          selector = ParseElementSelector( enumerator );
        }
        else
        {
          var right = ParseElementSelector( enumerator );
          if ( right == null )
            throw FormatError( enumerator );

          selector = CreateCasecadingSelector( selector, ' ', right );
        }
      }
    }

    private static FormatException FormatError( CharEnumerator enumerator )
    {
      return new FormatException( string.Format( "意外的字符 '{0}' ，在分析CSS选择器表达式 \"{1}\" 第 {2} 字符处。", enumerator.Current, enumerator.ToString(), enumerator.Offset ) );
    }


    private static FormatException FormatError( CharEnumerator enumerator, char desired )
    {
      return new FormatException( string.Format( "意外的字符 '{0}' ，在分析CSS选择器表达式 \"{1}\" 第 {2} 字符处，期望的字符为 '{3} '。", enumerator.Current, enumerator.ToString(), enumerator.Offset, desired ) );
    }


    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="combanitor">结合符</param>
    /// <param name="rightSelector">右选择器</param>
    /// <returns>层叠选择器</returns>
    private static ICssSelector CreateCasecadingSelector( ICssSelector leftSelector, char combanitor, CssElementSelector rightSelector )
    {
      return CssCasecadingSelector.Create( leftSelector, combanitor, rightSelector );
    }




    /// <summary>
    /// 跳过当前位置所有的空白字符
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns></returns>
    private static bool SkipWhiteSpace( CharEnumerator enumerator )
    {
      var ch = enumerator.Current;
      if ( ch == '\u0020' || ch == '\u0009' || ch == '\u000A' || ch == '\u000D' || ch == '\u000C' )
      {
        while ( enumerator.MoveNext() )
        {
          ch = enumerator.Current;
          if ( ch == '\u0020' || ch == '\u0009' || ch == '\u000A' || ch == '\u000D' || ch == '\u000C' )
            continue;

          break;
        }

        return true;
      }

      else
        return false;
    }

    /// <summary>
    /// 解析元素选择器
    /// </summary>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    private static CssElementSelector ParseElementSelector( CharEnumerator enumerator )
    {
      var elementName = ParseName( enumerator );
      if ( elementName == null && enumerator.Current == '*' )
      {
        enumerator.MoveNext();
        elementName = "*";
      }

      var attributeSelectors = new List<CssAttributeSelector>();
      var psedoclassSelectors = new List<ICssPseudoClassSelector>();

      while ( true )//解析ID和类选择符
      {
        if ( enumerator.Current == '#' )
        {
          EnsureNext( enumerator );
          attributeSelectors.Add( CreateAttributeSelector( "id", "=", ParseName( enumerator ) ) );
        }

        else if ( enumerator.Current == '.' )
        {
          EnsureNext( enumerator );
          attributeSelectors.Add( CreateAttributeSelector( "class", "~=", ParseName( enumerator ) ) );
        }

        else
          break;
      }

      while ( true )//解析属性选择符
      {
        if ( enumerator.Current == '[' )
          attributeSelectors.Add( ParseAttributeSelector( enumerator ) );

        else
          break;
      }

      while ( true )//解析伪类选择符
      {
        if ( enumerator.Current == ':' )
          psedoclassSelectors.Add( ParsePsedoclassSelector( enumerator ) );

        else
          break;
      }


      if ( elementName != null || attributeSelectors.Any() || psedoclassSelectors.Any() )
        return CreateElementSelector( elementName, attributeSelectors.ToArray(), psedoclassSelectors.ToArray() );

      else
        return null;
    }

    /// <summary>
    /// 创建元素选择器
    /// </summary>
    /// <param name="elementName">元素名</param>
    /// <param name="cssAttributeSelectors">CSS属性选择器</param>
    /// <param name="cssPseudoClassSelectors">CSS伪类选择器</param>
    /// <returns></returns>
    private static CssElementSelector CreateElementSelector( string elementName, CssAttributeSelector[] cssAttributeSelectors, ICssPseudoClassSelector[] cssPseudoClassSelectors )
    {
      return new CssElementSelector( elementName, cssAttributeSelectors, cssPseudoClassSelectors );
    }


    /// <summary>
    /// 解析属性选择器
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns></returns>
    private static CssAttributeSelector ParseAttributeSelector( CharEnumerator enumerator )
    {
      if ( enumerator.Current != '[' )
        return null;

      EnsureNext( enumerator );

      var attriuteName = ParseName( enumerator );

      var ch = enumerator.Current;

      if ( ch == ']' )
      {
        enumerator.MoveNext();
        return CreateAttributeSelector( attriuteName );
      }


      string compare;

      if ( ch == '=' )
        compare = "=";

      else if ( ch == '^' || ch == '$' || ch == '~' || ch == '*' || ch == '!' )
      {
        if ( EnsureNext( enumerator ) != '=' )//比较符不正确
          throw FormatError( enumerator, '=' );

        compare = ch + "=";
      }

      else
        throw FormatError( enumerator );


      EnsureNext( enumerator );//比较符后面没有字符

      var value = ParseQuoteText( enumerator );//尝试解析引用字符串

      if ( value == null )
      {

        var offset = enumerator.Offset;

        while ( true )
        {
          if ( enumerator.Current == ']' )//遇到结束符
            break;

          EnsureNext( enumerator );//遇到结束符前结束
        }

        value = enumerator.SubString( offset, enumerator.Offset - offset );
      }

      else if ( enumerator.Current != ']' )//引用字符串结束位置不是结束符
        throw FormatError( enumerator, ']' );


      enumerator.MoveNext();
      return CreateAttributeSelector( attriuteName, compare, value );
    }

    /// <summary>
    /// 创建属性选择器
    /// </summary>
    /// <param name="attriuteName"></param>
    /// <returns></returns>
    private static CssAttributeSelector CreateAttributeSelector( string attriuteName )
    {
      return new CssAttributeSelector( attriuteName, null, null );
    }

    /// <summary>
    /// 创建属性选择器
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="compare"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static CssAttributeSelector CreateAttributeSelector( string attributeName, string compare, string value )
    {
      return new CssAttributeSelector( attributeName, compare, value );
    }


    /// <summary>
    /// 解析伪类选择器
    /// </summary>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    private static ICssPseudoClassSelector ParsePsedoclassSelector( CharEnumerator enumerator )
    {
      if ( enumerator.Current != ':' )
        return null;

      EnsureNext( enumerator );

      var startOffset = enumerator.Offset;

      var name = ParseName( enumerator );

      if ( enumerator.Current != '(' )
        throw FormatError( enumerator, '(' );

      var i = 1;
      var argsOffset = enumerator.Offset + 1;

      while ( enumerator.MoveNext() )
      {
        if ( enumerator.Current == '(' )
          i++;

        else if ( enumerator.Current == ')' )
          i--;

        if ( i == 0 )
        {
          enumerator.MoveNext();

          var args = enumerator.SubString( argsOffset, enumerator.Offset - argsOffset - 1 );
          var expression = enumerator.SubString( startOffset, enumerator.Offset - startOffset );

          return CssPseudoClassSelectors.Create( name, args, expression );
        }

      }

      throw new FormatException( "意外的遇到字符串的结束" );
    }




    /// <summary>
    /// 确保当前位置尚未达到字符串末尾
    /// </summary>
    /// <param name="enumerator"></param>
    private static char EnsureNext( CharEnumerator enumerator )
    {
      if ( !enumerator.MoveNext() )
        throw new FormatException( "解析选择器时意外的遇到字符串末尾" );

      return enumerator.Current;
    }

    /// <summary>
    /// 解析引用字符串
    /// </summary>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    private static string ParseQuoteText( CharEnumerator enumerator )
    {
      var quoteCharactor = enumerator.Current;

      if ( quoteCharactor != '\'' && quoteCharactor != '\"' )
        return null;

      var offset = enumerator.Offset + 1;

      while ( true )
      {
        EnsureNext( enumerator );

        if ( enumerator.Current == quoteCharactor )
        {
          enumerator.MoveNext();
          return enumerator.SubString( offset, enumerator.Offset - offset - 1 );
        }
      }
    }

    /// <summary>
    /// 解析名称
    /// </summary>
    /// <param name="enumerator"></param>
    /// <returns></returns>
    private unsafe static string ParseName( CharEnumerator enumerator )
    {
      char* buffer = stackalloc char[100];
      int i = 0;

      do
      {
        var ch = enumerator.Current;

        if ( ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '-' )
          buffer[i++] = ch;

        else if ( ch == '|' )
          buffer[i++] = ':';

        else
          break;

      } while ( enumerator.MoveNext() );

      if ( i == 0 )
        return null;

      else
        return new string( buffer, 0, i );
    }



    private class CharEnumerator : IEnumerator<char>
    {


      private string _str;

      public CharEnumerator( string str )
      {
        _str = str;
      }

      private int _index;

      public char Current
      {
        get
        {
          if ( _index == _str.Length )
            return char.MinValue;

          return _str[_index];
        }
      }

      public void Dispose()
      {
      }

      object System.Collections.IEnumerator.Current
      {
        get { return Current; }
      }

      public bool MoveNext()
      {
        _index++;
        if ( _index < _str.Length )
          return true;

        if ( _index > _str.Length )
          _index = _str.Length;
        return false;
      }

      public void Reset()
      {
        _index = 0;
      }

      public int Offset
      {
        get { return _index; }
      }

      public string SubString( int offset, int length )
      {
        return _str.Substring( offset, length );
      }

      public override string ToString()
      {
        return _str;
      }
    }


  }
}
