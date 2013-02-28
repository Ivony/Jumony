using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  internal sealed class FreeComment : FreeNode, IFreeComment
  {
    public FreeComment( IHtmlFragment fragment, HtmlNodeFactory factory )
      : base( fragment, factory )
    {
      Comment = Node as IHtmlComment;
      if ( Comment == null )
        throw new InvalidOperationException();
    }


    private IHtmlComment Comment
    {
      get;
      set;
    }



    string IHtmlComment.Comment
    {
      get { return Comment.Comment; }
    }
  }
}
