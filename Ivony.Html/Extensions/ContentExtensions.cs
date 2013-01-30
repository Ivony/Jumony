using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

      if ( container == null )
        throw new ArgumentNullException( "container" );

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
      if ( node == null )
        throw new ArgumentNullException( "node" );


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

      var specialNode = node as IHtmlSpecial;//对于特殊标签无条件原样输出。
      if ( specialNode != null )
        return specialNode.RawHtml;

      var commentNode = node as IHtmlComment;
      if ( commentNode != null )
        return string.Format( CultureInfo.InvariantCulture, "<!--{0}-->", commentNode.Comment );

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

      throw new NotSupportedException();
    }

    /// <summary>
    /// 尝试生成元素的HTML表现形式
    /// </summary>
    /// <param name="element">要生成HTML的元素</param>
    /// <returns></returns>
    private static string GenerateElementHtml( IHtmlElement element )
    {
      var builder = new StringBuilder();

      if ( HtmlSpecification.selfCloseTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
      {
        if ( element.Nodes().Any() )
          throw new FormatException( string.Format( CultureInfo.InvariantCulture, "HTML元素 {0} 不能有任何内容", element.Name ) );

        builder.Append( GenerateTagHtml( element, true ) );
      }
      else
      {
        builder.Append( GenerateTagHtml( element, false ) );

        element.Nodes().ForAll( node => builder.Append( GenerateHtml( node ) ) );

        builder.AppendFormat( "</{0}>", element.Name );
      }
      return builder.ToString();

    }


    /// <summary>
    /// 尝试生成元素开始标签的HTML形式
    /// </summary>
    /// <param name="element">要生成HTML的元素</param>
    /// <param name="selfClosed">指示是否应产生自结束符号</param>
    /// <returns></returns>
    public static string GenerateTagHtml( IHtmlElement element, bool selfClosed )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );


      var builder = new StringBuilder( 20 );

      builder.Append( "<" );
      builder.Append( element.Name );

      foreach ( var attribute in element.Attributes() )
      {
        builder.Append( " " );
        builder.Append( attribute.Name );
        if ( attribute.AttributeValue != null )
        {
          if ( (HtmlSpecification.IsUriValue( attribute ) || HtmlSpecification.IsScriptValue( attribute )) && !attribute.AttributeValue.Contains( '"' ) )
            builder.Append( "=\"" ).Append( attribute.AttributeValue ).Append( "\"" );
          else
            builder.Append( "=\"" ).Append( HtmlEncoding.HtmlAttributeEncode( attribute.AttributeValue ) ).Append( "\"" );
        }
      }

      if ( selfClosed )
        builder.Append( " /" );

      builder.Append( ">" );
      return builder.ToString();

    }




    private static readonly Regex whitespaceRegex = new Regex( @"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    /// <summary>
    /// 尝试获取节点的文本表现形式，对于某些不支持文本表现形式的元素，将直接返回null
    /// </summary>
    /// <param name="node">要获取文本表现形式的节点</param>
    /// <returns></returns>
    public static string InnerText( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      var textNode = node as IHtmlTextNode;
      if ( textNode != null )
      {

        var parent = textNode.Parent();
        if ( parent == null )
          throw new InvalidOperationException();

        if ( HtmlSpecification.cdataTags.Contains( parent.Name, StringComparer.OrdinalIgnoreCase ) )
          return textNode.HtmlText;

        else if ( HtmlSpecification.preformatedElements.Contains( parent.Name, StringComparer.OrdinalIgnoreCase ) )
          return HtmlEncoding.HtmlDecode( textNode.HtmlText );

        else
          return HtmlEncoding.HtmlDecode( whitespaceRegex.Replace( textNode.HtmlText, " " ) );
      }

      var commentNode = node as IHtmlComment;
      if ( commentNode != null )
        return null;

      var element = node as IHtmlElement;
      if ( element != null )
      {
        if ( element.Name.EqualsIgnoreCase( "br" ) )
          return Environment.NewLine;

        else if ( HtmlSpecification.nonTextElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
          return null;
      }

      var container = node as IHtmlContainer;

      if ( container != null )
        return string.Join( "", container.Nodes().Select( n => InnerText( n ) ).ToArray() );

      throw new NotSupportedException();
    }


    /// <summary>
    /// 判断一个文本节点是不是全部由空白字符组成
    /// </summary>
    /// <param name="textNode">要判断的文本节点</param>
    /// <returns>是否全部是空白字符</returns>
    public static bool IsWhiteSpace( this IHtmlTextNode textNode )
    {

      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );


      if ( whitespaceRegex.Match( textNode.HtmlText ).Length == textNode.HtmlText.Length )
        return true;

      else
        return false;
    }

    /// <summary>
    /// 将文档呈现为 HTML
    /// </summary>
    /// <param name="document">要呈现的文档</param>
    /// <returns>文档的 HTML 形式</returns>
    public static string Render( this IHtmlDocument document )
    {
      using ( var writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        Render( document, writer, null );

        return writer.ToString();
      }
    }


    /// <summary>
    /// 将文档呈现为 HTML
    /// </summary>
    /// <param name="document">要呈现的文档</param>
    /// <param name="adapters">HTML 输出转换器</param>
    /// <returns>文档的 HTML 形式</returns>
    public static string Render( this IHtmlDocument document, params IHtmlRenderAdapter[] adapters )
    {
      using ( var writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        Render( document, writer, adapters );

        return writer.ToString();
      }
    }



    /// <summary>
    /// 将文档呈现为 HTML
    /// </summary>
    /// <param name="document">要呈现的文档</param>
    /// <param name="writer">HTML 编写器</param>
    /// <param name="adapters">HTML 输出转换器</param>
    public static void Render( this IHtmlDocument document, TextWriter writer, params IHtmlRenderAdapter[] adapters )
    {

      if ( document == null )
        throw new ArgumentNullException( "document" );

      RenderChilds( document, new HtmlRenderContext( writer, adapters ) );
    }


    /// <summary>
    /// 将文档呈现为 HTML
    /// </summary>
    /// <param name="document">要呈现的文档</param>
    /// <param name="stream">用于输出呈现的 HTML 的流</param>
    /// <param name="encoding">呈现的 HTML 的编码格式</param>
    public static void Render( this IHtmlDocument document, Stream stream, Encoding encoding )
    {
      using ( var writer = new StreamWriter( stream, encoding ) )
      {
        Render( document, writer );

        writer.Flush();
      }
    }


    public static void RenderChilds( this IHtmlContainer container, TextWriter writer, params IHtmlRenderAdapter[] adapters )
    {
      RenderChilds( container, new HtmlRenderContext( writer, adapters ) );
    }


    public static void RenderChilds( this IHtmlContainer container, HtmlRenderContext context )
    {
      foreach ( var node in container.Nodes() )
      {
        Render( node, context );
      }
    }

    /// <summary>
    /// 将节点呈现为 HTML
    /// </summary>
    /// <param name="node">要呈现的节点</param>
    /// <returns>呈现的 HTML</returns>
    public static string Render( this IHtmlNode node )
    {
      using ( var writer = new StringWriter( CultureInfo.InvariantCulture ) )
      {
        Render( node, writer );

        return writer.ToString();
      }
    }


    /// <summary>
    /// 将节点呈现为 HTML
    /// </summary>
    /// <param name="node">要呈现的节点</param>
    /// <param name="writer">HTML 编写器</param>
    public static void Render( this IHtmlNode node, TextWriter writer )
    {
      Render( node, writer, null );
    }


    /// <summary>
    /// 将节点呈现为 HTML
    /// </summary>
    /// <param name="node">要呈现的节点</param>
    /// <param name="writer">HTML 编写器</param>
    /// <param name="adapters">HTML 输出转换器</param>
    public static void Render( this IHtmlNode node, TextWriter writer, params IHtmlRenderAdapter[] adapters )
    {
      Render( node, new HtmlRenderContext( writer, adapters ) );
    }



    /// <summary>
    /// 将节点呈现为 HTML
    /// </summary>
    /// <param name="node">要呈现的节点</param>
    /// <param name="context">渲染上下文</param>
    public static void Render( this IHtmlNode node, HtmlRenderContext context )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );


      foreach ( var a in context.Adapters )
      {
        if ( a.Render( node, context ) )
          return;
      }


      var renderable = node as IHtmlRenderableNode;

      if ( renderable != null )
      {
        renderable.Render( context );
        return;
      }

      var element = node as IHtmlElement;
      if ( element != null )
      {
        RenderElementAndChilds( element, context );
        return;
      }

      context.Write( node.OuterHtml() );
    }


    /// <summary>
    /// 渲染元素和其子节点
    /// </summary>
    /// <param name="element">要渲染的元素</param>
    /// <param name="context">渲染上下文</param>
    private static void RenderElementAndChilds( IHtmlElement element, HtmlRenderContext context )
    {

      var writer = context.Writer;

      if ( HtmlSpecification.selfCloseTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
      {
        var builder = new StringBuilder();

        builder.Append( GenerateTagHtml( element, true ) );

        if ( element.Nodes().Any() )
          throw new FormatException( string.Format( CultureInfo.InvariantCulture, "HTML元素 {0} 不能有任何内容", element.Name ) );


        writer.Write( builder );
      }
      else
      {

        writer.Write( GenerateTagHtml( element, false ) );
        RenderChilds( element, context );
        writer.Write( "</{0}>", element.Name );

      }
    }


  }


  /// <summary>
  /// 文本格式化选项
  /// </summary>
  public enum TextFormatOption
  {
    /// <summary>不进行任何格式化</summary>
    None,
    /// <summary>将换行转换为 &lt;br /&gt;</summary>
    BreakLine,
    /// <summary>将换行转换为 &lt;br /&gt;并且保持空白字符不被合并。</summary>
    BreakLineAndKeepWhiteSpace
  }

}
