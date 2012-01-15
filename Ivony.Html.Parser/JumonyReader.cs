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

    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})|(?<doctype>{4})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern, Regulars.doctypeDeclarationPattern );

    /// <summary>
    /// 用于匹配 HTML 标签的正则表达式对象
    /// </summary>
    protected static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );


    private static bool _isWarmedUp = false;

    /// <summary>
    /// 调用此方法通知进行预热 JumonyReader
    /// </summary>
    public static void WarmUp()
    {
      if ( !_isWarmedUp )
      {
        tagRegex.IsMatch( "" );
        _isWarmedUp = true;
      }
    }


    /// <summary>
    /// 创建一个 JumonyReader 对象
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
    /// 枚举读取到的每一个内容元素
    /// </summary>
    /// <returns>枚举结果</returns>
    public IEnumerable<HtmlContentFragment> EnumerateContent()
    {

      var index = 0;//读取指针

      while ( true )
      {

        HtmlContentFragment contentNode;

        //CData标签处理

        if ( CDataElement != null )//如果在CData标签内。
        {
          contentNode = FindEndTag( index, CDataElement );
          CDataElement = null;//自动退出 CData 元素读取模式
        }

        else
          contentNode = NextContentNode( index );


        if ( contentNode == null )
        {
          //处理末尾的文本
          if ( index != HtmlText.Length )
            yield return CreateText( index, HtmlText.Length );

          yield break;
        }

        else//当读取到了某个节点
        {
          if ( index < contentNode.StartIndex )
            yield return CreateText( index, contentNode.StartIndex );

          yield return contentNode;
        }

        index = contentNode.StartIndex + contentNode.Length;//推后读取指针
      }
    }


    /// <summary>
    /// 查找指定元素的结束标签（用于CData元素结束位置查找）
    /// </summary>
    /// <param name="index">查找的开始位置</param>
    /// <param name="elementName">元素名称</param>
    /// <returns>找到的结束标签，若已到达文档末尾，则返回 null</returns>
    protected virtual HtmlEndTag FindEndTag( int index, string elementName )
    {

      Regex endTagRegex = HtmlSpecification.GetEndTagRegex( elementName );
      var endTagMatch = endTagRegex.Match( HtmlText, index );


      if ( !endTagMatch.Success )
        return null;


      return new HtmlEndTag( CreateFragment( endTagMatch ), elementName );
    }


    /// <summary>
    /// 读取下一个 HTML 内容节点（开始标签、结束标签、注释或特殊节点）
    /// </summary>
    /// <param name="index">读取开始位置</param>
    /// <returns>下一个内容节点，若已经达到文档末尾，则返回 null</returns>
    protected virtual HtmlContentFragment NextContentNode( int index )
    {


      var match = tagRegex.Match( HtmlText, index );

      if ( !match.Success )//如果不再有标签的匹配
        return null;



      if ( match.Groups["beginTag"].Success )
        return CreateBeginTag( match );

      else if ( match.Groups["endTag"].Success )
        return CreateEndTag( match );

      else if ( match.Groups["comment"].Success )
        return CreateComment( match );

      else if ( match.Groups["special"].Success )
        return CreateSpacial( match );

      else if ( match.Groups["doctype"].Success )
        return CreateDoctypeDeclaration( match );

      else
        throw new InvalidOperationException();
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


    /// <summary>
    /// 创建属性设置内容对象
    /// </summary>
    /// <param name="match">属性设置的匹配</param>
    /// <returns>HTML 属性设置的内容对象</returns>
    protected virtual IEnumerable<HtmlAttributeSetting> CreateAttributes( Match match )
    {
      foreach ( Capture capture in match.Groups["attribute"].Captures )
      {
        string name = capture.FindCaptures( match.Groups["attrName"] ).Single().Value;
        string value = capture.FindCaptures( match.Groups["attrValue"] ).Select( c => c.Value ).SingleOrDefault();

        yield return new HtmlAttributeSetting( CreateFragment( capture ), name, value );
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



    private HtmlContentFragment CreateDoctypeDeclaration( Match match )
    {
      var raw = match.ToString();

      var fragment = CreateFragment( match );
      return new HtmlDoctypeDeclaration( fragment );
    }




    protected virtual HtmlTextContent CreateText( int startIndex, int endIndex )
    {
      var text = new HtmlTextContent( new HtmlContentFragment( this, startIndex, endIndex - startIndex ) );
      return text;
    }

    protected HtmlContentFragment CreateFragment( Capture capture )
    {
      return new HtmlContentFragment( this, capture.Index, capture.Length );
    }

  }
}
