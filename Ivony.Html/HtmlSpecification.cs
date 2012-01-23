using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Ivony.Fluent;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 规范（依据 HTML 4.01规范）
  /// </summary>
  public static class HtmlSpecification
  {

    /// <summary>所有 CDATA 元素，其内部文本不被当作 HTML 文本解释</summary>
    public static readonly ICollection<string> cdataTags = new ReadOnlyCollection<string>( new[] { "script", "code", "style", "textarea", "title" } );
    /// <summary>所有自结束元素，没有内容和结束标签</summary>
    public static readonly ICollection<string> selfCloseTags = new ReadOnlyCollection<string>( new[] { "area", "base", "basefont", "br", "col", "frame", "hr", "img", "input", "isindex", "link", "meta", "param", "wbr", "bgsound", "spacer", "keygen" } );

    /// <summary>所有可选结束元素，其在何处结束由 ImmediatelyClose 方法确定</summary>
    public static readonly ICollection<string> optionalCloseTags = new ReadOnlyCollection<string>( new[] { "body", "colgroup", "dd", "dt", "head", "html", "li", "option", "p", "tbody", "td", "tfoot", "th", "thead", "tr" } );



    /// <summary>所有设置字体和样式的元素</summary>
    public static readonly ICollection<string> fontstyleElements = new ReadOnlyCollection<string>( new[] { "tt", "i", "b", "big", "small" } );
    /// <summary>所有界定文本段落的元素</summary>
    public static readonly ICollection<string> phrasElements = new ReadOnlyCollection<string>( new[] { "em", "strong", "dfn", "code", "samp", "kbd", "var", "cite", "abbr", "acronym" } );
    /// <summary>所有用于特殊目的的 HTML 元素</summary>
    public static readonly ICollection<string> specialElements = new ReadOnlyCollection<string>( new[] { "a", "img", "object", "br", "script", "map", "q", "sub", "sup", "span", "bdo" } );
    /// <summary>所有表单控件元素</summary>
    public static readonly ICollection<string> formcontrolElements = new ReadOnlyCollection<string>( new[] { "input", "select", "textarea", "label", "button" } );

    /// <summary>所有行内呈现的元素</summary>
    public static readonly ICollection<string> inlineElements = new ReadOnlyCollection<string>( fontstyleElements.Union( phrasElements ).Union( specialElements ).Union( formcontrolElements ).ToArray() );


    /// <summary>所有定义章节标题元素</summary>
    public static readonly ICollection<string> headingElements = new ReadOnlyCollection<string>( new[] { "h1", "h2", "h3", "h4", "h5", "h6" } );
    /// <summary>所有定义列表的元素</summary>
    public static readonly ICollection<string> listElements = new ReadOnlyCollection<string>( new[] { "ul", "ol" } );
    /// <summary>预格式化元素</summary>
    public static readonly ICollection<string> preformatedElements = new ReadOnlyCollection<string>( new[] { "pre" } );

    /// <summary>所有块级元素</summary>
    public static readonly ICollection<string> blockElements = new ReadOnlyCollection<string>( headingElements.Union( listElements ).Union( preformatedElements ).Union( new[] { "p", "dl", "div", "noscript", "blockquote", "form", "hr", "table", "fieldset", "address" } ).ToArray() );

    /// <summary>所有文本流元素</summary>
    public static readonly ICollection<string> flowElements = new ReadOnlyCollection<string>( blockElements.Union( inlineElements ).ToArray() );

    /// <summary>所有非显示文本元素</summary>
    public static readonly ICollection<string> nonTextElements = new ReadOnlyCollection<string>( new[] { "table", "tr", "input", "style", "title", "map", "head", "meta", "script", "br", "frame" } );





    /// <summary>
    /// 判断一个属性值的值是否应被视为URI。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为URI</returns>
    public static bool IsUriValue( IHtmlAttribute attribute )
    {

      if ( attribute == null )
        throw new ArgumentNullException( "attribute" );


      var elementName = attribute.Element.Name;

      switch ( attribute.Name.ToLowerInvariant() )
      {
        case "action":
          return elementName.EqualsIgnoreCase( "form" );

        case "background":
          return elementName.EqualsIgnoreCase( "body" );

        case "cite":
          return elementName.EqualsIgnoreCase( "blockquote" )
            || elementName.EqualsIgnoreCase( "q" )
            || elementName.EqualsIgnoreCase( "del" )
            || elementName.EqualsIgnoreCase( "ins" );

        case "classid":
          return elementName.EqualsIgnoreCase( "object" );

        case "codebase":
          return elementName.EqualsIgnoreCase( "object" )
            || elementName.EqualsIgnoreCase( "applet" );

        case "data":
          return elementName.EqualsIgnoreCase( "object" );

        case "href":
          return elementName.EqualsIgnoreCase( "a" )
            || elementName.EqualsIgnoreCase( "area" )
            || elementName.EqualsIgnoreCase( "link" )
            || elementName.EqualsIgnoreCase( "base" );


        case "longdesc":
          return elementName.EqualsIgnoreCase( "img" )
            || elementName.EqualsIgnoreCase( "frame" )
            || elementName.EqualsIgnoreCase( "iframe" );

        case "profile":
          return elementName.EqualsIgnoreCase( "head" );

        case "src":
          return elementName.EqualsIgnoreCase( "script" )
            || elementName.EqualsIgnoreCase( "input" )
            || elementName.EqualsIgnoreCase( "frame" )
            || elementName.EqualsIgnoreCase( "iframe" )
            || elementName.EqualsIgnoreCase( "img" );

        default:
          return false;

      }
    }


    /// <summary>
    /// 判断一个属性值的值是否应被视为脚本。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为脚本</returns>
    public static bool IsScriptValue( IHtmlAttribute attribute )
    {
      if ( attribute == null )
        throw new ArgumentNullException( "attribute" );


      var elementName = attribute.Element.Name;

      switch ( attribute.Name.ToLowerInvariant() )
      {
        case "onblur":
        case "onchange":
        case "onclick":
        case "ondbclick":
        case "onfocus":
        case "onkeydown":
        case "onkeypress":
        case "onkeyup":
        case "onload":
        case "onmousedown":
        case "onmousemove":
        case "onmouseout":
        case "onmouseover":
        case "onmouseup":
        case "onreset":
        case "onselect":
        case "onsubmit":
        case "onunload":
          return true;
      }



      return false;
    }







    /// <summary>
    /// 检查可选结束标签在当前位置是否需要立即关闭
    /// </summary>
    /// <param name="openTag">当前开放的可选结束标签</param>
    /// <param name="nextTag">HTML 分析器遇到的下一个标签</param>
    /// <returns>是否需要立即关闭</returns>
    public static bool ImmediatelyClose( string openTag, string nextTag )
    {

      if ( openTag == null )
        throw new ArgumentNullException( "openTag" );

      if ( nextTag == null )
        throw new ArgumentNullException( "nextTag" );


      openTag = openTag.ToLowerInvariant();
      nextTag = nextTag.ToLowerInvariant();


      switch ( openTag )
      {
        case "colgroup":
          return nextTag != "col";

        case "dd":
        case "dt":
          return nextTag == "dd" || nextTag == "dt";

        case "li":
          return nextTag == "li";

        case "option":
          return nextTag == "option";

        case "p":
          return blockElements.Contains( nextTag );//因为上面已经转换为小写形式，所以这里不必执行不区分大小写的比较。

        case "thead":
        case "tbody":
        case "tfoot":
          return new[] { "thead", "tbody", "tfoot" }.Contains( nextTag );

        case "tr":
          return new[] { "tr", "thead", "tbody", "tfoot" }.Contains( nextTag );

        case "td":
        case "th":
          return new[] { "tr", "td", "th", "thead", "tbody", "tfoot" }.Contains( nextTag );

        case "head":
          return nextTag == "body";

        default:
          return false;
      }
    }



    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.OrdinalIgnoreCase );

    private static object _sync = new object();


    private static bool _isWarmedUp = false;
    public static void WarmUp()
    {
      if ( _isWarmedUp )
        return;

      lock ( _sync )
      {
        tagNameRegex.IsMatch( "" );

        foreach ( var name in cdataTags )
        {
          GetEndTagRegex( name ).IsMatch( "" );
        }
      }
    }


    /// <summary>
    /// 用于匹配元素标签名的正则
    /// </summary>
    public static readonly Regex tagNameRegex = new Regex( @"^[\w:\.]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant );



    /// <summary>
    /// 获取匹配指定结束标签的正则表达式对象
    /// </summary>
    /// <param name="tagName">标签名</param>
    /// <returns>匹配指定结束标签的正则表达式对象</returns>
    public static Regex GetEndTagRegex( string tagName )
    {

      if ( tagName == null )
        throw new ArgumentNullException( "tagName" );

      if ( !tagNameRegex.IsMatch( tagName ) )
        throw new ArgumentException( string.Format( CultureInfo.InvariantCulture, "\"{0}\" 不是一个合法有效的 HTML 元素名称", tagName ), "tagName" );


      tagName = tagName.ToLowerInvariant();

      lock ( _sync )
      {
        Regex regex;

        if ( !endTagRegexes.TryGetValue( tagName, out regex ) )
          endTagRegexes.Add( tagName, regex = new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant ) );

        return regex;
      }
    }






    internal static readonly IDictionary<string, char> entities = new Dictionary<string, char> 
    {
      { "quot",'\"' },
      { "amp",'&' },
      { "apos",'\'' },
      { "lt",'<' },
      { "gt",'>' },
      { "nbsp", '\x00a0' },
      { "iexcl", '\x00a1' },
      { "cent", '\x00a2' },
      { "pound", '\x00a3' },
      { "curren", '\x00a4' },
      { "yen", '\x00a5' },
      { "brvbar", '\x00a6' },
      { "sect", '\x00a7' },
      { "uml", '\x00a8' },
      { "copy", '\x00a9' },
      { "ordf", '\x00aa' },
      { "laquo", '\x00ab' },
      { "not", '\x00ac' },
      { "shy", '\x00ad' },
      { "reg", '\x00ae' },
      { "macr", '\x00af' },
      { "deg", '\x00b0' },
      { "plusmn", '\x00b1' },
      { "sup2", '\x00b2' },
      { "sup3", '\x00b3' },
      { "acute", '\x00b4' },
      { "micro", '\x00b5' },
      { "para", '\x00b6' },
      { "middot", '\x00b7' },
      { "cedil", '\x00b8' },
      { "sup1", '\x00b9' },
      { "ordm", '\x00ba' },
      { "raquo", '\x00bb' },
      { "frac14", '\x00bc' },
      { "frac12", '\x00bd' },
      { "frac34", '\x00be' },
      { "iquest", '\x00bf' },
      { "Agrave", '\x00c0' },
      { "Aacute", '\x00c1' },
      { "Acirc", '\x00c2' },
      { "Atilde", '\x00c3' },
      { "Auml", '\x00c4' },
      { "Aring", '\x00c5' },
      { "AElig", '\x00c6' },
      { "Ccedil", '\x00c7' },
      { "Egrave", '\x00c8' },
      { "Eacute", '\x00c9' },
      { "Ecirc", '\x00ca' },
      { "Euml", '\x00cb' },
      { "Igrave", '\x00cc' },
      { "Iacute", '\x00cd' },
      { "Icirc", '\x00ce' },
      { "Iuml", '\x00cf' },
      { "ETH", '\x00d0' },
      { "Ntilde", '\x00d1' },
      { "Ograve", '\x00d2' },
      { "Oacute", '\x00d3' },
      { "Ocirc", '\x00d4' },
      { "Otilde", '\x00d5' },
      { "Ouml", '\x00d6' },
      { "times", '\x00d7' },
      { "Oslash", '\x00d8' },
      { "Ugrave", '\x00d9' },
      { "Uacute", '\x00da' },
      { "Ucirc", '\x00db' },
      { "Uuml", '\x00dc' },
      { "Yacute", '\x00dd' },
      { "THORN", '\x00de' },
      { "szlig", '\x00df' },
      { "agrave", '\x00e0' },
      { "aacute", '\x00e1' },
      { "acirc", '\x00e2' },
      { "atilde", '\x00e3' },
      { "auml", '\x00e4' },
      { "aring", '\x00e5' },
      { "aelig", '\x00e6' },
      { "ccedil", '\x00e7' },
      { "egrave", '\x00e8' },
      { "eacute", '\x00e9' },
      { "ecirc", '\x00ea' },
      { "euml", '\x00eb' },
      { "igrave", '\x00ec' },
      { "iacute", '\x00ed' },
      { "icirc", '\x00ee' },
      { "iuml", '\x00ef' },
      { "eth", '\x00f0' },
      { "ntilde", '\x00f1' },
      { "ograve", '\x00f2' },
      { "oacute", '\x00f3' },
      { "ocirc", '\x00f4' },
      { "otilde", '\x00f5' },
      { "ouml", '\x00f6' },
      { "divide", '\x00f7' },
      { "oslash", '\x00f8' },
      { "ugrave", '\x00f9' },
      { "uacute", '\x00fa' },
      { "ucirc", '\x00fb' },
      { "uuml", '\x00fc' },
      { "yacute", '\x00fd' },
      { "thorn", '\x00fe' },
      { "yuml", '\x00ff' },
      { "OElig", 'Œ' },
      { "oelig", 'œ' },
      { "Scaron", 'Š' },
      { "scaron", 'š' },
      { "Yuml", 'Ÿ' },
      { "fnof", 'ƒ' },
      { "circ", 'ˆ' },
      { "tilde", '˜' },
      { "Alpha", 'Α' },
      { "Beta", 'Β' },
      { "Gamma", 'Γ' },
      { "Delta", 'Δ' },
      { "Epsilon", 'Ε' },
      { "Zeta", 'Ζ' },
      { "Eta", 'Η' },
      { "Theta", 'Θ' },
      { "Iota", 'Ι' },
      { "Kappa", 'Κ' },
      { "Lambda", 'Λ' },
      { "Mu", 'Μ' },
      { "Nu", 'Ν' },
      { "Xi", 'Ξ' },
      { "Omicron", 'Ο' },
      { "Pi", 'Π' },
      { "Rho", 'Ρ' },
      { "Sigma", 'Σ' },
      { "Tau", 'Τ' },
      { "Upsilon", 'Υ' },
      { "Phi", 'Φ' },
      { "Chi", 'Χ' },
      { "Psi", 'Ψ' },
      { "Omega", 'Ω' },
      { "alpha", 'α' },
      { "beta", 'β' },
      { "gamma", 'γ' },
      { "delta", 'δ' },
      { "epsilon", 'ε' },
      { "zeta", 'ζ' },
      { "eta", 'η' },
      { "theta", 'θ' },
      { "iota", 'ι' },
      { "kappa", 'κ' },
      { "lambda", 'λ' },
      { "mu", 'μ' },
      { "nu", 'ν' },
      { "xi", 'ξ' },
      { "omicron", 'ο' },
      { "pi", 'π' },
      { "rho", 'ρ' },
      { "sigmaf", 'ς' },
      { "sigma", 'σ' },
      { "tau", 'τ' },
      { "upsilon", 'υ' },
      { "phi", 'φ' },
      { "chi", 'χ' },
      { "psi", 'ψ' },
      { "omega", 'ω' },
      { "thetasym", 'ϑ' },
      { "upsih", 'ϒ' },
      { "piv", 'ϖ' },
      { "ensp", ' ' },
      { "emsp", ' ' },
      { "thinsp", ' ' },
      { "zwnj", '‌' },
      { "zwj", '‍' },
      { "lrm", '‎' },
      { "rlm", '‏' },
      { "ndash", '–' },
      { "mdash", '—' },
      { "lsquo", '‘' },
      { "rsquo", '’' },
      { "sbquo", '‚' },
      { "ldquo", '“' },
      { "rdquo", '”' },
      { "bdquo", '„' },
      { "dagger", '†' },
      { "Dagger", '‡' },
      { "bull", '•' },
      { "hellip", '…' },
      { "permil", '‰' },
      { "prime", '′' },
      { "Prime", '″' },
      { "lsaquo", '‹' },
      { "rsaquo", '›' },
      { "oline", '‾' },
      { "frasl", '⁄' },
      { "euro", '€' },
      { "image", 'ℑ' },
      { "weierp", '℘' },
      { "real", 'ℜ' },
      { "trade", '™' },
      { "alefsym", 'ℵ' },
      { "larr", '←' },
      { "uarr", '↑' },
      { "rarr", '→' },
      { "darr", '↓' },
      { "harr", '↔' },
      { "crarr", '↵' },
      { "lArr", '⇐' },
      { "uArr", '⇑' },
      { "rArr", '⇒' },
      { "dArr", '⇓' },
      { "hArr", '⇔' },
      { "forall", '∀' },
      { "part", '∂' },
      { "exist", '∃' },
      { "empty", '∅' },
      { "nabla", '∇' },
      { "isin", '∈' },
      { "notin", '∉' },
      { "ni", '∋' },
      { "prod", '∏' },
      { "sum", '∑' },
      { "minus", '−' },
      { "lowast", '∗' },
      { "radic", '√' },
      { "prop", '∝' },
      { "infin", '∞' },
      { "ang", '∠' },
      { "and", '∧' },
      { "or", '∨' },
      { "cap", '∩' },
      { "cup", '∪' },
      { "int", '∫' },
      { "there4", '∴' },
      { "sim", '∼' },
      { "cong", '≅' },
      { "asymp", '≈' },
      { "ne", '≠' },
      { "equiv", '≡' },
      { "le", '≤' },
      { "ge", '≥' },
      { "sub", '⊂' },
      { "sup", '⊃' },
      { "nsub", '⊄' },
      { "sube", '⊆' },
      { "supe", '⊇' },
      { "oplus", '⊕' },
      { "otimes", '⊗' },
      { "perp", '⊥' },
      { "sdot", '⋅' },
      { "lceil", '⌈' },
      { "rceil", '⌉' },
      { "lfloor", '⌊' },
      { "rfloor", '⌋' },
      { "lang", '〈' },
      { "rang", '〉' },
      { "loz", '◊' },
      { "spades", '♠' },
      { "clubs", '♣' },
      { "hearts", '♥' },
      { "diams", '♦' } 
    };

    internal static readonly ICollection<char> _htmlEntityEndingChars = new ReadOnlyCollection<char>( new char[] { ';', '&' } );
  }

}
