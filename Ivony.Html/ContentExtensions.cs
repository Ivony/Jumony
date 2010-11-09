using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 提供内容分析呈现的扩展方法
  /// </summary>
  public static class ContentExtensions
  {

    /// <summary>
    /// 尝试获取元素子节点的HTML表现形式，如果DOM不支持RawHtml，则此方法尝试生成HTML
    /// </summary>
    /// <param name="container">要生成InnerHTML的容器</param>
    /// <returns>容器所有子节点的HTML表现形式</returns>
    public static string InnerHtml( this IHtmlContainer container )
    {
      return InnerHtml( container, false );
    }

    /// <summary>
    /// 尝试获取元素子节点的HTML表现形式
    /// </summary>
    /// <param name="container">要生成InnerHTML的容器</param>
    /// <param name="normalization">指定是否强制生成规范化的HTML，如果此参数为true，将忽略DOM节点的RawHtml属性</param>
    /// <returns>容器所有子节点的HTML表现形式</returns>
    public static string InnerHtml( this IHtmlContainer container, bool normalization )
    {
      StringBuilder builder = new StringBuilder();

      foreach ( var node in container.Nodes() )
      {
        builder.Append( OuterHtml( node, normalization ) );
      }

      return builder.ToString();
    }

    /// <summary>
    /// 尝试获取整个节点的HTML表现形式，如果DOM不支持RawHtml，则此方法尝试生成HTML
    /// </summary>
    /// <param name="node">要获取HTML表现形式的节点</param>
    /// <returns></returns>
    public static string OuterHtml( this IHtmlNode node )
    {
      return OuterHtml( node, false );
    }


    /// <summary>
    /// 尝试获取整个节点的HTML表现形式
    /// </summary>
    /// <param name="node">要获取HTML表现形式的节点</param>
    /// <param name="normalization">指定是否强制生成规范化的HTML，如果此参数为true，将忽略DOM节点的RawHtml属性</param>
    /// <returns></returns>
    public static string OuterHtml( this IHtmlNode node, bool normalization )
    {
      var raw = node.RawHtml;
      if ( raw == null || normalization )
        return GenerateHtml( node );
      else
        return raw;
    }



    /// <summary>
    /// 尝试生成节点的HTML表现形式
    /// </summary>
    /// <param name="node">要生成HTML的节点</param>
    /// <returns></returns>
    private static string GenerateHtml( IHtmlNode node )
    {
      var textNode = node as IHtmlTextNode;

      if ( textNode != null )
      {
        return textNode.HtmlText;
      }

      var specialNode = node as IHtmlSpecial;//对于特殊节点原样输出。
      if ( specialNode != null )
        return specialNode.RawHtml;

      var commentNode = node as IHtmlComment;
      if ( commentNode != null )
        return string.Format( "<!--{0}-->", commentNode.Comment );

      var element = node as IHtmlElement;
      if ( element != null )
        return GenerateElementHtml( element );

      var document = node as IHtmlDocument;
      if ( document != null )
      {
        var builder = new StringBuilder();
        document.Nodes().ForAll( child => builder.Append( GenerateHtml( child ) ) );
        return builder.ToString();
      }

      throw new InvalidOperationException();
    }

    /// <summary>
    /// 尝试生成元素的HTML表现形式
    /// </summary>
    /// <param name="element">要生成HTML的元素</param>
    /// <returns></returns>
    private static string GenerateElementHtml( IHtmlElement element )
    {
      var builder = new StringBuilder();

      builder.Append( GenerateTagHtml( element ) );

      if ( HtmlSpecification.selfCloseTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )
      {
        if ( element.Nodes().Any() )
          throw new FormatException( string.Format( "HTML元素 {0} 不能有任何内容", element.Name ) );

        builder.Insert( builder.Length - 1, " /" );//在末尾插入自结束标志“/”。
      }
      else
      {
        element.Nodes().ForAll( node => builder.Append( GenerateHtml( node ) ) );

        builder.AppendFormat( "</{0}>", element.Name );
      }
      return builder.ToString();

    }


    /// <summary>
    /// 尝试生成元素开始标签的HTML形式
    /// </summary>
    /// <param name="element">要生成HTML的元素</param>
    /// <returns></returns>
    private static string GenerateTagHtml( IHtmlElement element )
    {
      var builder = new StringBuilder();

      builder.Append( "<" );
      builder.Append( element.Name );

      foreach ( var attribute in element.Attributes() )
      {
        builder.Append( " " );
        builder.Append( attribute.Name );
        if ( attribute.AttributeValue != null )
          builder.AppendFormat( "=\"{0}\"", HtmlEncoding.HtmlAttributeEncode( attribute.AttributeValue ) );
      }

      builder.Append( ">" );
      return builder.ToString();

    }




    private static readonly string[] noTextElements = new[] { "table", "tr", "input", "style", "head", "meta", "script", "br", "frame" };

    private static readonly Regex whitespaceRegex = new Regex( @"\s+", RegexOptions.Compiled );

    /// <summary>
    /// 尝试获取节点的文本表现形式，对于某些不支持文本表现形式的元素，将直接返回null
    /// </summary>
    /// <param name="node">要获取文本表现形式的节点</param>
    /// <returns></returns>
    public static string InnerText( this IHtmlNode node )
    {
      var textNode = node as IHtmlTextNode;
      if ( textNode != null )
        return HtmlEncoding.HtmlDecode( whitespaceRegex.Replace( textNode.HtmlText, " " ) );

      var commentNode = node as IHtmlComment;
      if ( commentNode != null )
        return null;

      var element = node as IHtmlElement;
      if ( element != null )
      {
        if ( element.Name.EqualsIgnoreCase( "br" ) )
          return Environment.NewLine;

        else if ( noTextElements.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )
          return null;
      }

      var container = node as IHtmlContainer;

      return string.Join( "", container.Nodes().Select( n => InnerText( n ) ).ToArray() );
    }


    /// <summary>
    /// 判断一个文本节点是不是全部由空白字符组成
    /// </summary>
    /// <param name="textNode">要判断的文本节点</param>
    /// <returns>是否全部是空白字符</returns>
    public static bool IsWhiteSpace( this IHtmlTextNode textNode )
    {
      if ( whitespaceRegex.Match( textNode.HtmlText ).Length == textNode.HtmlText.Length )
        return true;

      else
        return false;
    }

  }
}
