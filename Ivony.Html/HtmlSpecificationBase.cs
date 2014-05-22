using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 规范抽象基类
  /// </summary>
  public abstract class HtmlSpecificationBase
  {

    /// <summary>
    /// 确认一个元素是否为 CData 元素
    /// </summary>
    /// <remarks>
    /// 对于 CData 元素，解析器不会将开始标签到结束标签之间的任何符号视为 HTML 符号的一部分。
    /// PCData 元素同时也是 CData 元素
    /// </remarks>
    /// <param name="elementName">元素名</param>
    /// <returns>是否为 CData 元素</returns>
    public abstract bool IsCDataElement( string elementName );

    /// <summary>
    /// 确认一个元素是否拥有可选的结束标签
    /// </summary>
    /// <param name="elementName">元素名</param>
    /// <returns>是否拥有可选的结束标签</returns>
    public abstract bool IsOptionalEndTag( string elementName );

    /// <summary>
    /// 确认一个元素是否不能拥有结束标签和子元素
    /// </summary>
    /// <param name="elementName">元素名</param>
    /// <returns>是否不能拥有结束标签和子元素</returns>
    public abstract bool IsForbiddenEndTag( string elementName );

    /// <summary>
    /// 检查可选结束标签在当前位置是否需要立即关闭
    /// </summary>
    /// <param name="openTag">当前开放的可选结束标签</param>
    /// <param name="nextTag">HTML 分析器遇到的下一个标签</param>
    /// <returns>是否需要立即关闭</returns>
    public abstract bool ImmediatelyClose( string openTag, string nextTag );



    /// <summary>
    /// 判断元素是否为块级元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为块级元素</returns>
    public abstract bool IsBlockElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为行内元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为行内元素</returns>
    public abstract bool IsInlineElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为特殊元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为特殊元素</returns>
    public abstract bool IsSpecialElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为表单输入元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为表单输入元素</returns>
    public abstract bool IsFormInputElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为样式设置元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为样式设置元素</returns>
    public abstract bool IsStylingElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为列表定义元素
    /// </summary>
    /// <param name="element">需要判断的元素</param>
    /// <returns>是否为样式设置元素</returns>
    public abstract bool IsListElement( IHtmlElement element );

    /// <summary>
    /// 判断元素是否为段落内容定义元素
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <returns>是否为段落内容定义元素</returns>
    public abstract bool IsPhraseElement( IHtmlElement element );



    /// <summary>
    /// 判断一个属性值的值是否应被视为URI。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为URI</returns>
    public abstract bool IsUriValue( IHtmlAttribute attribute );

    /// <summary>
    /// 判断一个属性值的值是否应被视为脚本。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>其值是否应被视为脚本</returns>
    public abstract bool IsScriptValue( IHtmlAttribute attribute );

    /// <summary>
    /// 判断一个属性是否为标记属性。
    /// </summary>
    /// <param name="attribute">要检查的属性</param>
    /// <returns>是否为标记属性</returns>
    public abstract bool IsMarkupAttribute( IHtmlAttribute attribute );




    /// <summary>
    /// 确认元素文本内容格式
    /// </summary>
    /// <param name="element">要确认文本内容格式的元素</param>
    /// <returns>文本内容格式</returns>
    public abstract TextMode ElementTextMode( IHtmlElement element );


    /// <summary>
    /// 获取与之相匹配的 CSS 样式规范
    /// </summary>
    /// <returns></returns>
    public abstract CssStyleSpecificationBase GetCssStyleSpecification();


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


  /// <summary>
  /// 文本模式
  /// </summary>
  public enum TextMode
  {
    /// <summary>普通，内容当作 HTML 来解释</summary>
    Normal,
    /// <summary>CData，内容当作文本来解释</summary>
    CData,
    /// <summary>预格式化，不合并空白字符</summary>
    Preformated,
    /// <summary>非文本，元素没有文本内容</summary>
    NonText
  }


}
