using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Globalization;
using Ivony.Html.Styles;

namespace Ivony.Html
{
  /// <summary>
  /// CSS 分析器，用于分析 CSS 选择器表达式
  /// </summary>
  public static class CssParser
  {

    /// <summary>
    /// 从选择器表达式创建选择器对象
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>CSS 选择器对象</returns>
    public static ISelector ParseSelector( string expression )
    {

      if ( expression == null )
        return null;

      var selector = selectorsCache[expression] as ISelector;
      if ( selector != null )
        return selector;

      using ( var enumerator = new CharEnumerator( expression ) )
      {
        selector = ParseSelector( enumerator );

        selectorsCache[expression] = selector;

        return selector;
      }
    }

    private static Hashtable selectorsCache = new Hashtable();




    /// <summary>
    /// 从选择器表达式创建元素选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>CSS 元素选择器对象</returns>
    public static CssElementSelector ParseElementSelector( string expression )
    {
      if ( expression == null )
        return null;

      using ( var enumerator = new CharEnumerator( expression ) )
      {
        return ParseElementSelector( enumerator );
      }
    }



    /// <summary>
    /// 创建带范畴限定的选择器
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <param name="scope">范畴限定，上溯时不超出此范畴</param>
    /// <returns>带范畴限定的层叠选择器</returns>
    public static ISelector Create( IHtmlContainer scope, string expression )
    {
      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      if ( expression == null )
        throw new ArgumentNullException( "expression" );

      return CssCasecadingSelector.Create( new CssAncetorRelativeSelector( new ContainerRestrict( scope ) ), CssParser.ParseSelector( expression ) );
    }



    /// <summary>
    /// 解析并创建选择器对象
    /// </summary>
    /// <param name="enumerator">用于读取选择器表达式的枚举器</param>
    /// <returns>选择器对象</returns>
    private static ISelector ParseSelector( CharEnumerator enumerator )
    {


      var selectorList = new List<ISelector>();

      ISelector selector = null;

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
      return new FormatException( string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析CSS选择器表达式 \"{1}\" 第 {2} 字符处。", enumerator.Current, enumerator.ToString(), enumerator.Offset ) );
    }


    private static FormatException FormatError( CharEnumerator enumerator, char desired )
    {
      return new FormatException( string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析CSS选择器表达式 \"{1}\" 第 {2} 字符处，期望的字符为 '{3}' 。", enumerator.Current, enumerator.ToString(), enumerator.Offset, desired ) );
    }


    /// <summary>
    /// 创建层叠选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="combanitor">结合符</param>
    /// <param name="rightSelector">右选择器</param>
    /// <returns>层叠选择器</returns>
    private static ISelector CreateCasecadingSelector( ISelector leftSelector, char combanitor, CssElementSelector rightSelector )
    {
      return CssCasecadingSelector.Create( leftSelector, combanitor, rightSelector );
    }




    /// <summary>
    /// 跳过当前位置所有的空白字符
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns>是否跳过了任何空白字符</returns>
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
    /// <param name="enumerator">字符枚举器</param>
    /// <returns>解析好的元素选择器</returns>
    private static CssElementSelector ParseElementSelector( CharEnumerator enumerator )
    { 
      CssSpecificity specificity;
      return ParseElementSelector( enumerator, out specificity );
    }


    /// <summary>
    /// 解析元素选择器
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns>解析好的元素选择器</returns>
    private static CssElementSelector ParseElementSelector( CharEnumerator enumerator, out CssSpecificity specificity )
    {

      int a = 0, b = 0, c = 0;

      var elementName = ParseName( enumerator );
      if ( elementName.IsNullOrEmpty() && enumerator.Current == '*' )
      {
        enumerator.MoveNext();
        elementName = "*";
      }
      else
        c++;

      var attributeSelectors = new List<CssAttributeSelector>();
      var psedoclassSelectors = new List<ICssPseudoClassSelector>();

      while ( true )//解析ID和类选择符
      {
        if ( enumerator.Current == '#' )
        {
          EnsureNext( enumerator );
          attributeSelectors.Add( CreateAttributeSelector( "id", "=", ParseName( enumerator ) ) );
          a++;
        }

        else if ( enumerator.Current == '.' )
        {
          EnsureNext( enumerator );
          attributeSelectors.Add( CreateAttributeSelector( "class", "~=", ParseName( enumerator ) ) );
          b++;
        }

        else
          break;
      }

      while ( true )//解析属性选择符
      {
        if ( enumerator.Current == '[' )
        {
          attributeSelectors.Add( ParseAttributeSelector( enumerator ) );
          b++;
        }
        else
          break;
      }

      while ( true )//解析伪类选择符
      {
        if ( enumerator.Current == ':' )
        {
          psedoclassSelectors.Add( ParsePsedoclassSelector( enumerator ) );
          b++;
        }
        else
          break;
      }


      specificity = new CssSpecificity( a, b, c );


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
    /// <returns>元素选择器</returns>
    private static CssElementSelector CreateElementSelector( string elementName, CssAttributeSelector[] cssAttributeSelectors, ICssPseudoClassSelector[] cssPseudoClassSelectors )
    {
      return new CssElementSelector( elementName, cssAttributeSelectors, cssPseudoClassSelectors );
    }


    /// <summary>
    /// 解析属性选择器
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns>属性选择器</returns>
    private static CssAttributeSelector ParseAttributeSelector( CharEnumerator enumerator )
    {
      if ( enumerator.Current != '[' )
        return null;

      EnsureNext( enumerator );

      var attriuteName = ParseName( enumerator );

      if ( attriuteName.IsNullOrEmpty() )
        throw FormatError( enumerator );

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
    /// <param name="attriuteName">属性名</param>
    /// <returns>属性选择器，此选择器确定属性是否存在</returns>
    private static CssAttributeSelector CreateAttributeSelector( string attriuteName )
    {
      return new CssAttributeSelector( attriuteName, null, null );
    }

    /// <summary>
    /// 创建属性选择器
    /// </summary>
    /// <param name="attributeName">属性名</param>
    /// <param name="compare">比较符</param>
    /// <param name="value">属性参考值</param>
    /// <returns>属性选择器</returns>
    private static CssAttributeSelector CreateAttributeSelector( string attributeName, string compare, string value )
    {
      return new CssAttributeSelector( attributeName, compare, value );
    }


    /// <summary>
    /// 解析伪类选择器
    /// </summary>
    /// <param name="enumerator">字符枚举器</param>
    /// <returns></returns>
    private static ICssPseudoClassSelector ParsePsedoclassSelector( CharEnumerator enumerator )
    {
      if ( enumerator.Current != ':' )
        return null;

      EnsureNext( enumerator );

      var startOffset = enumerator.Offset;

      var name = ParseName( enumerator );
      if ( name.IsNullOrEmpty() )
        throw FormatError( enumerator );

      if ( name.EqualsIgnoreCase( "not" ) )
        return ParseNegationPseudoClass( enumerator );


      if ( enumerator.Current != '(' )
        return CreatePseudoClassSelector( name );

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

          return CreatePseudoClassSelector( name, args );
        }

      }

      throw new FormatException( "意外的遇到字符串的结束" );
    }

    /// <summary>
    /// 解析否定伪类
    /// </summary>
    /// <returns></returns>
    private static ICssPseudoClassSelector ParseNegationPseudoClass( CharEnumerator enumerator )
    {

      if ( enumerator.Current != '(' )
        throw FormatError( enumerator, '(' );

      EnsureNext( enumerator );

      SkipWhiteSpace( enumerator );
      var elementSelector = ParseElementSelector( enumerator );
      SkipWhiteSpace( enumerator );

      if ( enumerator.Current != ')' )
        throw FormatError( enumerator, ')' );
      enumerator.MoveNext();

      return new NegationPseudoClass( elementSelector );

    }


    private static ICssPseudoClassSelector CreatePseudoClassSelector( string name )
    {

      return CreatePseudoClassSelector( name, null );

    }


    private static ICssPseudoClassSelector CreatePseudoClassSelector( string name, string args )
    {
      ICssPseudoClassProvider provider;
      if ( !_providers.TryGetValue( name, out provider ) )
        throw new FormatException( string.Format( CultureInfo.InvariantCulture, "无法识别的伪类 {0} ，是否未注册伪类提供程序？", name ) );

      return provider.CreateSelector( name, args );
    }



    private static readonly IDictionary<string, ICssPseudoClassProvider> _providers = new Dictionary<string, ICssPseudoClassProvider>( StringComparer.OrdinalIgnoreCase );
    private static object _sync = new object();


    static CssParser()
    {
      InternalPseudoClassProvider.RegisterInternalPseudoClasses();
    }


    /// <summary>
    /// 注册自定义 CSS 伪类选择器提供程序
    /// </summary>
    /// <param name="name">伪类名</param>
    /// <param name="provider">伪类选择器提供程序</param>
    public static void RegisterPseudoClassProvider( string name, ICssPseudoClassProvider provider )
    {
      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( provider == null )
        throw new ArgumentNullException( "provider" );


      lock ( _sync )
      {
        if ( _providers.ContainsKey( name ) )
          throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "系统中已经存在提供 \"{0}\" 的伪类的提供程序", name ) );

        _providers.Add( name, provider );
      }
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
    private static string ParseName( CharEnumerator enumerator )
    {

      bool flag = false;//标识字符串中是否存在"|"
      int offset = enumerator.Offset;

      do
      {
        var ch = enumerator.Current;

        if ( ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '-' || ch == '_' )
          continue;

        else if ( ch == '|' )
        {
          flag = true;
          continue;
        }

        else
          break;

      } while ( enumerator.MoveNext() );


      if ( offset == enumerator.Offset )
        return null;

      var name = enumerator.SubString( offset, enumerator.Offset - offset );
      if ( flag )
        name = name.Replace( '|', ':' );

      return name;
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

      public void Skip( int length )
      {
        _index += length;
      }

    }
  }
}
