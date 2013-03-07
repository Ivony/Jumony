using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public class CommentAdapter : NodeAdapter, IHtmlComment
  {
    private IHTMLCommentElement _comment;
    private IHTMLCommentElement2 _comment2;

    public CommentAdapter( object node )
      : base( node )
    {
      _comment = node as IHTMLCommentElement;
      _comment2 = node as IHTMLCommentElement2;
    }

    public override string RawHtml
    {
      get { return null; }
    }

    public string Comment
    {
      get { return _comment.text; }
    }
  }
}
