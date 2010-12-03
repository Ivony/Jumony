using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlCommentObject : HtmlContentFragment
  {

    public HtmlCommentObject( HtmlContentFragment fragment, string comment )
      : base( fragment )
    {
      Comment = comment;
    }

    public string Comment
    {
      get;
      private set;
    }

  }
}
