using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Ivony.Html.MSHTMLAdapter
{
  public static class ConvertExtensions
  {


    public static IHtmlNode AsNode( object node )
    {
      if ( node is mshtml.IHTMLElement )
        return new ElementAdapter( node );

      else if ( node is mshtml.IHTMLDOMTextNode )
        return new TextNodeAdapter( node );

      else if ( node is mshtml.IHTMLCommentElement )
        return new CommentAdapter( node );
    }

  }
}
