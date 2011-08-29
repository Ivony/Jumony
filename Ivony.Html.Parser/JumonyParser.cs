using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using Ivony.Html.Parser.ContentModels;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 一个标准 HTML 解析器的实现（基于HTML 4.01规范）
  /// </summary>
  public class JumonyParser : HtmlParserBase
  {

    protected override IHtmlDomProvider Provider
    {
      get { return DomProvider.Instance; }
    }

    protected override IHtmlReader CreateReader( string html )
    {
      return new JumonyReader( html );
    }

    protected override void ProcessEndTagMissingBeginTag( HtmlEndTag endTag )
    {
      //忽略多出的结束标签
    }


    protected override IHtmlTextNode ProcessText( HtmlTextContent textContent )
    {
      var textNode = base.ProcessText( textContent ).CastTo<DomTextNode>();
      textNode.ContentFragment = textContent;
      return textNode;
    }

    protected override IHtmlComment ProcessComment( HtmlCommentContent commentContent )
    {
      var commentObject = base.ProcessComment( commentContent ).CastTo<DomComment>();
      commentObject.ContentFragment = commentContent;
      return commentObject;
    }

    protected override IHtmlSpecial ProcessSpecial( HtmlSpecialTag specialTag )
    {
      var specialObject = base.ProcessSpecial( specialTag ).CastTo<DomSpecial>();
      specialObject.ContentFragment = specialTag;
      return specialObject;
    }


    /*
    public override IHtmlDocument Parse( string html, Uri url )
    {
      var document = base.Parse( html, url ).CastTo<DomDocument>();
      document.ContentFragment = new HtmlContentFragment( Reader, 0, Reader.HtmlText.Length );
      return document;
    }
    */

    private static bool _isWarmedUp = false;

    /// <summary>
    /// 调用此方法通知预热 JumonyParser
    /// </summary>
    public static void WarmUp()
    {
      if ( !_isWarmedUp )
      {
        JumonyReader.WarmUp();
        _isWarmedUp = true;
      }
    }


  }
}
