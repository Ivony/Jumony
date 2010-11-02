using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivony.Html.Parser
{
  public class JumonyParser
  {

    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern );

    private static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled );


    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.InvariantCultureIgnoreCase );


    static JumonyParser()
    {
      foreach ( var tagName in HtmlSpecification.cdataTags )//为所有的CDATA标签准备匹配结束标记的正则表达式
      {
        endTagRegexes.Add( tagName, new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled ) );
      }
    }



    private Stack<DomContainer> containerStack = new Stack<DomContainer>();



    public DomDocument Parse( string html )
    {


      var document = new DomDocument();
      containerStack.Push( document );


      int index = 0;


      while ( true )
      {

        //CData标签处理

        var element = containerStack.Peek() as DomElement;

        if ( element != null && HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )//如果在CData标签内。
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
        new DomTextNode( containerStack.Peek(), html.Substring( index ) );

      return document;
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
        new DomTextNode( containerStack.Peek(), text );

      index = match.Index + match.Length;
      return index;
    }



    private void ProcessBeginTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;
      bool selfClosed = match.Groups["selfClosed"].Success;

      if ( HtmlSpecification.selfCloseTags.Contains( tagName, StringComparer.InvariantCultureIgnoreCase ) )
        selfClosed = true;


      //检查父标签是否可选结束标记，并相应处理
      {
        var element = containerStack.Peek() as DomElement;
        if ( element != null && HtmlSpecification.optionalCloseTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )
        {
          if ( HtmlSpecification.ImmediatelyClose( element.Name, tagName ) )
            containerStack.Pop();
        }
      }



      //处理所有属性
      var attributes = new Dictionary<string, string>();

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
        var element = new DomElement( containerStack.Peek(), tagName, attributes );
        CompleteNode( element );


        if ( !selfClosed )
          containerStack.Push( element );
      }
    }


    private void ProcessEndTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;



      if ( containerStack.OfType<DomElement>().Select( e => e.Name ).Contains( tagName, StringComparer.InvariantCultureIgnoreCase ) )
      {
        while ( true )
        {
          var element = containerStack.Pop() as DomElement;
          if ( element.Name.Equals( tagName, StringComparison.InvariantCultureIgnoreCase ) )
            break;
        }
      }
      else//如果堆栈中没有对应的开始标签，则将这个结束标签解释为文本
      {
        CompleteNode( new DomTextNode( containerStack.Peek(), match.Value ) );
      }
    }

    private void ProcessComment( Match match )
    {
      CompleteNode( new DomComment( containerStack.Peek(), match.Groups["commentText"].Value ) );
    }


    private void ProcessSpecial( Match match )
    {
      CompleteNode( new DomSpecial( containerStack.Peek(), match.Value ) );
    }


    private void CompleteNode( DomNode node )
    {

    }



  }
}
