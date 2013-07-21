using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ivony.Fluent;



#pragma warning disable 1591


namespace Ivony.Html
{
  /// <summary>
  /// 定义 HTML 4.01 规范
  /// </summary>
  /// <remarks>此类型实例是只读且线程安全的</remarks>
  public sealed class Html41Specification : HtmlSpecificationBase
  {

    /// <summary>所有 CDATA 元素，其内部文本不被当作 HTML 文本解释</summary>
    public static readonly ICollection<string> cdataTags = new ReadOnlyCollection<string>( new[] { "script", "style", "textarea", "title" } );
    /// <summary>所有自结束元素，没有内容和结束标签</summary>
    public static readonly ICollection<string> fobiddenEndTags = new ReadOnlyCollection<string>( new[] { "area", "base", "basefont", "br", "col", "frame", "hr", "img", "input", "isindex", "link", "meta", "param", "wbr", "bgsound", "spacer", "keygen" } );

    /// <summary>所有可选结束元素，其在何处结束由 ImmediatelyClose 方法确定</summary>
    public static readonly ICollection<string> optionalCloseTags = new ReadOnlyCollection<string>( new[] { "body", "colgroup", "dd", "dt", "head", "html", "li", "option", "p", "tbody", "td", "tfoot", "th", "thead", "tr" } );



    /// <summary>所有设置字体和样式的元素</summary>
    public static readonly ICollection<string> stylingElements = new ReadOnlyCollection<string>( new[] { "tt", "i", "b", "big", "small" } );
    /// <summary>所有界定文本段落的元素</summary>
    public static readonly ICollection<string> phraseElements = new ReadOnlyCollection<string>( new[] { "em", "strong", "dfn", "code", "samp", "kbd", "var", "cite", "abbr", "acronym" } );
    /// <summary>所有用于特殊目的的 HTML 元素</summary>
    public static readonly ICollection<string> specialElements = new ReadOnlyCollection<string>( new[] { "a", "img", "object", "br", "script", "map", "q", "sub", "sup", "span", "bdo" } );
    /// <summary>所有表单控件元素</summary>
    public static readonly ICollection<string> inputControlElements = new ReadOnlyCollection<string>( new[] { "input", "select", "textarea", "label", "button" } );

    /// <summary>所有行内呈现的元素</summary>
    public static readonly ICollection<string> inlineElements = new ReadOnlyCollection<string>( stylingElements.Union( phraseElements ).Union( specialElements ).Union( inputControlElements ).ToArray() );


    /// <summary>所有定义章节标题元素</summary>
    public static readonly ICollection<string> headingElements = new ReadOnlyCollection<string>( new[] { "h1", "h2", "h3", "h4", "h5", "h6" } );
    /// <summary>所有定义列表的元素</summary>
    public static readonly ICollection<string> listElements = new ReadOnlyCollection<string>( new[] { "ul", "ol", "dl" } );
    /// <summary>预格式化元素</summary>
    public static readonly ICollection<string> preformatedElements = new ReadOnlyCollection<string>( new[] { "pre" } );

    /// <summary>所有块级元素</summary>
    public static readonly ICollection<string> blockElements = new ReadOnlyCollection<string>( headingElements.Union( listElements ).Union( preformatedElements ).Union( new[] { "p", "div", "noscript", "blockquote", "form", "hr", "table", "fieldset", "address" } ).ToArray() );

    /// <summary>所有文本流元素</summary>
    public static readonly ICollection<string> flowElements = new ReadOnlyCollection<string>( blockElements.Union( inlineElements ).ToArray() );

    /// <summary>所有非显示文本元素</summary>
    public static readonly ICollection<string> nonTextElements = new ReadOnlyCollection<string>( new[] { "table", "tr", "input", "style", "title", "map", "head", "meta", "script", "br", "frame" } );




    public override bool IsCDataElement( string elementName )
    {
      return cdataTags.Contains( elementName, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsOptionalEndTag( string elementName )
    {
      return optionalCloseTags.Contains( elementName, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsForbiddenEndTag( string elementName )
    {
      return fobiddenEndTags.Contains( elementName, StringComparer.OrdinalIgnoreCase );
    }

    public override bool ImmediatelyClose( string openTag, string nextTag )
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

    public override bool IsBlockElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return blockElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsInlineElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return inlineElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsSpecialElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return specialElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsFormInputElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return inputControlElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsStylingElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return stylingElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsListElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return listElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }

    public override bool IsPhraseElement( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      return phraseElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase );
    }




    public override bool IsUriValue( IHtmlAttribute attribute )
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

    public override bool IsScriptValue( IHtmlAttribute attribute )
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

    public override bool IsMarkupAttribute( IHtmlAttribute attribute )
    {
      switch ( attribute.Name.ToLowerInvariant() )
      {
        case "checked":
        case "compact":
        case "declare":
        case "defer":
        case "disabled":
        case "ismap":
        case "multiple":
        case "nohref":
        case "noresize":
        case "noshade":
        case "nowrap":
        case "readonly":
        case "selected":
          return true;
      }

      return false;
    }


    public override TextMode ElementTextMode( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( element.AncestorsAndSelf().Any( e => preformatedElements.Contains( e.Name, StringComparer.OrdinalIgnoreCase ) ) )
        return TextMode.Preformated;

      else if ( cdataTags.Contains( element.Name ) )
        return TextMode.CData;

      else if ( nonTextElements.Contains( element.Name ) )
        return TextMode.NonText;

      else
        return TextMode.Normal;
    }

    public override CssStyleSpecificationBase GetCssStyleSpecification()
    {
      return new Css21StyleSpecification();
    }
  }
}
