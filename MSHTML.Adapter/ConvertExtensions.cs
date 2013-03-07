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

      var _node = node as IHTMLDOMNode;

      if ( _node == null )
        return null;



      var type = _node.nodeType;

      if ( type == 8 )
        return new CommentAdapter( node );

      else if ( type == 1 )
        return new ElementAdapter( node );

      else if ( type == 3 )
        return new TextNodeAdapter( node );

      return null;
    }

    public static IHtmlDocument AsDocument( object document )
    {
      if ( document is mshtml.HTMLDocument )
        return new DocumentAdapter( document );

      return null;
    }



    internal static IHtmlAttribute AsAttribute( object attribute, ElementAdapter element )
    {
      return new AttributeAdapter( attribute, element );
    }
  }
}
