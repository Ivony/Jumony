using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  
  /// <summary>
  /// 旧版本的 HTML 解析
  /// </summary>
  public class JumonyLegacyParser : IHtmlParser
  {

    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern );

    private static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );


    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.OrdinalIgnoreCase );


    static JumonyLegacyParser()
    {
      foreach ( var tagName in HtmlSpecification.cdataTags )//为所有的CDATA标签准备匹配结束标记的正则表达式
      {
        endTagRegexes.Add( tagName, new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant ) );
      }
    }



    private Stack<IDomContainer> containerStack = new Stack<IDomContainer>();



    public virtual DomDocument Parse( string html, Uri url )
    {
      if ( html == null )
        throw new ArgumentNullException( "html" );

      if ( url != null && !url.IsAbsoluteUri )
        throw new ArgumentException( "必须是绝对URI", "url" );


      var document = new DomDocument( url );

      if ( string.IsNullOrEmpty( html ) )
        return document;

      containerStack.Push( document );

      ParseInternal( html );

      return document;
    }

    protected void ParseInternal( string html )
    {
      int index = 0;


      while ( true )
      {

        //CData标签处理

        var element = containerStack.Peek() as DomElement;

        if ( element != null && HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )//如果在CData标签内。
        {

          Regex endTagRegex = endTagRegexes[element.Name];
          var endTagMatch = endTagRegex.Match( html, index );


          //处理文本节点
          index = ProcessText( html, index, endTagMatch );


          containerStack.Pop();
          continue;

        }


        var match = tagRegex.Match( html, index );

        if ( !match.Success )//如果不再有标签的匹配
          break;


        //处理文本节点
        index = ProcessText( html, index, match );



        if ( match.Groups["beginTag"].Success )
          ProcessBeginTag( match );
        else if ( match.Groups["endTag"].Success )
          ProcessEndTag( match );
        else if ( match.Groups["comment"].Success )
          ProcessComment( match );
        else if ( match.Groups["special"].Success )
          ProcessSpecial( match );
        else
          throw new InvalidOperationException();
      }

      if ( index != html.Length )
        CreateTextNode( html.Substring( index ) );
    }


    /// <summary>
    /// 处理文本节点
    /// </summary>
    /// <param name="html">正在分析的HTML文本</param>
    /// <param name="index">文本节点开始位置</param>
    /// <param name="match">下一个匹配（非文本节点）</param>
    private int ProcessText( string html, int index, Match match )
    {
      var text = html.Substring( index, match.Index - index );
      if ( text.Length > 0 )
        CreateTextNode( text );

      index = match.Index + match.Length;
      return index;
    }


    private void ProcessBeginTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;
      bool selfClosed = match.Groups["selfClosed"].Success;

      if ( HtmlSpecification.selfCloseTags.Contains( tagName, StringComparer.OrdinalIgnoreCase ) )
        selfClosed = true;


      //检查父标签是否可选结束标记，并相应处理
      {
        var element = containerStack.Peek() as DomElement;
        if ( element != null && HtmlSpecification.optionalCloseTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          if ( HtmlSpecification.ImmediatelyClose( element.Name, tagName ) )
            containerStack.Pop();
        }
      }



      //处理所有属性
      var attributes = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["attribute"].Captures )
      {
        string name = capture.FindCaptures( match.Groups["attrName"] ).Single().Value;
        string value = capture.FindCaptures( match.Groups["attrValue"] ).Select( c => c.Value ).SingleOrDefault();

        value = HtmlEncoding.HtmlDecode( value );

        if ( attributes.ContainsKey( name ) )
          continue;

        attributes.Add( name, value );
      }


      //加入容器堆栈。
      {
        var element = CreateElement( tagName, attributes );


        if ( !selfClosed )
          containerStack.Push( element );
      }
    }



    private void ProcessEndTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;



      if ( containerStack.OfType<DomElement>().Select( e => e.Name ).Contains( tagName, StringComparer.OrdinalIgnoreCase ) )
      {
        while ( true )
        {
          var element = containerStack.Pop() as DomElement;
          if ( element.Name.EqualsIgnoreCase( tagName ) )
            break;
        }
      }
      else
      {
        ProcessEndTagMissingBeginTag( match );
      }
    }

    /// <summary>
    /// 处理丢失了开始标签的结束标签
    /// </summary>
    /// <param name="match"></param>
    protected virtual void ProcessEndTagMissingBeginTag( Match match )
    {
      //如果堆栈中没有对应的开始标签，则将这个结束标签解释为文本
      CreateTextNode( match.Value );
    }

    private void ProcessComment( Match match )
    {
      CreateCommet( match.Groups["commentText"].Value );
    }


    private void ProcessSpecial( Match match )
    {
      CreateSpecial( match.Value );
    }





    private DomElement CreateElement( string tagName, Dictionary<string, string> attributes )
    {
      var element = new DomElement( tagName, attributes );
      CompleteNode( element );
      return element;
    }

    private DomTextNode CreateTextNode( string text )
    {
      var node = new DomTextNode( text );
      CompleteNode( node );
      return node;
    }

    private DomComment CreateCommet( string comment )
    {
      var node = new DomComment( comment );
      CompleteNode( node );
      return node;
    }


    private DomSpecial CreateSpecial( string html )
    {
      var special = new DomSpecial( html );
      CompleteNode( special );
      return special;
    }




    protected virtual void CompleteNode( DomNode node )
    {
      var container = containerStack.Peek();
      container.NodeCollection.Add( node );
    }


    IHtmlDocument IHtmlParser.Parse( string html, Uri url )
    {
      return Parse( html, url );
    }

  }
}
