using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ivony.Html
{
  public static class XLINQExtensions
  {

    public static void Add( this IHtmlContainer container, params object[] childs )
    {
      foreach ( var node in childs.Cast<XNode>() )
      {
        var element = node as XElement;
        if ( element != null )
          container.Add( element );

        var text = node as XText;
        if ( text != null )
          container.Add( text );

        var comment = node as XComment;
        if ( comment != null )
          container.Add( comment );

      }
    }

    public static IHtmlElement Add(this IHtmlContainer container, XElement element )
    {
      var result = container.AddElement( element.Name.LocalName );
      foreach ( var attribute in element.Attributes() )
        result.SetAttribute( attribute.Name.LocalName, attribute.Value );

      return result;
    }

    public static IHtmlTextNode Add( this IHtmlContainer container, XText text )
    {
      return container.AddTextNode( HtmlEncoding.HtmlEncode( text.Value ) );
    }

    public static IHtmlComment Add( this IHtmlContainer container, XComment comment )
    {
      return container.AddComment( comment.Value );
    }

  }
}
