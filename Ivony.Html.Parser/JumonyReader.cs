using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser.ContentModels;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// Jumony 提供的HTML文档读取器的一个实现
  /// </summary>
  public class JumonyReader : IHtmlReader
  {

    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern );

    /// <summary>
    /// 用于匹配 HTML 标签的正则表达式对象
    /// </summary>
    protected static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );



    /// <summary>
    /// 创建一个 JumonyReader对象
    /// </summary>
    /// <param name="htmlText"></param>
    public JumonyReader( string htmlText )
    {
      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      HtmlText = htmlText;

      CDataElement = null;

    }

    /// <summary>
    /// 要分析的 HTML 文本
    /// </summary>
    public string HtmlText
    {
      get;
      private set;
    }


    /// <summary>
    /// 若当前处于 CData 元素内部，此属性指示元素名
    /// </summary>
    protected string CDataElement
    {
      get;
      private set;
    }


    void IHtmlReader.EnterCDataMode( string elementName )
    {
      CDataElement = elementName;
    }



    /// <summary>
    /// 当前所读取的位置
    /// </summary>
    protected int Index
    {
      get;
      private set;
    }



    /// <summary>
    /// 枚举读取到的每一个内容元素
    /// </summary>
    /// <returns>枚举结果</returns>
    public IEnumerable<HtmlContentFragment> EnumerateContent()
    {

      Index = 0;

      while ( true )
      {

        //CData标签处理

        if ( CDataElement != null )//如果在CData标签内。
        {

          Regex endTagRegex = HtmlSpecification.GetEndTagRegex( CDataElement );
          var endTagMatch = endTagRegex.Match( HtmlText, Index );


          if ( !endTagMatch.Success )
          {
            //处理末尾的文本
            if ( Index != HtmlText.Length )
              yield return CreateText( HtmlText.Length );


            yield break;
          }

          if ( endTagMatch.Index > Index )
            yield return CreateText( endTagMatch.Index );

          yield return new HtmlEndTag( CreateFragment( endTagMatch ), CDataElement );


          CDataElement = null;//自动退出 CData 元素读取模式

        }


        var match = tagRegex.Match( HtmlText, Index );

        if ( !match.Success )//如果不再有标签的匹配
        {
          //处理末尾的文本
          if ( Index != HtmlText.Length )
            yield return CreateText( HtmlText.Length );

          yield break;
        }


        //处理文本节点
        if ( match.Index > Index )
          yield return CreateText( match.Index );


        if ( match.Groups["beginTag"].Success )
          yield return CreateBeginTag( match );

        else if ( match.Groups["endTag"].Success )
          yield return CreateEndTag( match );

        else if ( match.Groups["comment"].Success )
          yield return CreateComment( match );

        else if ( match.Groups["special"].Success )
          yield return CreateSpacial( match );

        else
          throw new InvalidOperationException();
      }
    }


    /// <summary>
    /// 创建开始标签内容对象
    /// </summary>
    /// <param name="match">开始标签的匹配</param>
    /// <returns>开始标签内容对象</returns>
    protected virtual HtmlBeginTag CreateBeginTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;
      bool selfClosed = match.Groups["selfClosed"].Success;


      //处理所有属性
      var attributes = CreateAttributes( match );


      var fragment = CreateFragment( match );

      return new HtmlBeginTag( fragment, tagName, selfClosed, attributes );
    }


    protected virtual IEnumerable<HtmlAttributeSetting> CreateAttributes( Match match )
    {
      foreach ( Capture capture in match.Groups["attribute"].Captures )
      {
        string name = capture.FindCaptures( match.Groups["attrName"] ).Single().Value;
        string value = capture.FindCaptures( match.Groups["attrValue"] ).Select( c => c.Value ).SingleOrDefault();

        yield return new HtmlAttributeSetting( CreateFragment( capture, false ), name, value );
      }
    }

    protected virtual HtmlEndTag CreateEndTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;

      var fragment = CreateFragment( match );
      return new HtmlEndTag( fragment, tagName );
    }

    protected virtual HtmlCommentContent CreateComment( Match match )
    {
      var commentText = match.Groups["commentText"].Value;

      var fragment = CreateFragment( match );
      return new HtmlCommentContent( fragment, commentText );
    }


    protected virtual HtmlSpecialTag CreateSpacial( Match match )
    {
      var raw = match.ToString();
      var symbol = raw.Substring( 1, 1 );
      var content = match.Groups["specialText"].Value;

      var fragment = CreateFragment( match );
      return new HtmlSpecialTag( fragment, content, symbol );
    }




    protected virtual HtmlTextContent CreateText( int endIndex )
    {
      var text = new HtmlTextContent( new HtmlContentFragment( this, Index, endIndex - Index ) );
      Index = endIndex;

      return text;
    }

    protected HtmlContentFragment CreateFragment( Capture capture )
    {
      return CreateFragment( capture, true );
    }

    protected virtual HtmlContentFragment CreateFragment( Capture capture, bool setIndex )
    {
      if ( setIndex )
        Index = capture.Index + capture.Length;

      return new HtmlContentFragment( this, capture.Index, capture.Length );
    }
  }
}
