using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class Html5DraftSpecial
  {

    public interface IContentKind
    {
      bool IsBelong( IHtmlNode node );
    }




    public static readonly IContentKind MetadataContent = new MetadataContentKind();

    private class MetadataContentKind : IContentKind
    {
      public bool IsBelong( IHtmlNode node )
      {
        var element = node as IHtmlElement;

        switch ( element.Name )
        {
          case "base":
          case "command":
          case "link":
          case "meta":
          case "noscript":
          case "script":
          case "style":
          case "title":
            return true;

          default: return false;
        }

      }
    }






    public static readonly IContentKind FlowContent = new FlowContentKind();

    internal class FlowContentKind : IContentKind
    {
      public bool IsBelong( IHtmlNode node )
      {

        if ( node is IHtmlTextNode )
          return true;

        var element = node as IHtmlElement;

        if ( element == null )
          return false;

        switch ( element.Name.ToLowerInvariant() )
        {
          case "a":
          case "abbr":
          case "address":
          case "article":
          case "aside":
          case "audio":
          case "b":
          case "bdo":
          case "blockquote":
          case "br":
          case "button":
          case "canvas":
          case "cite":
          case "code":
          case "command":
          case "datalist":
          case "del":
          case "details":
          case "dfn":
          case "div":
          case "dl":
          case "em":
          case "embed":
          case "fieldset":
          case "figure":
          case "footer":
          case "form":
          case "h1":
          case "h2":
          case "h3":
          case "h4":
          case "h5":
          case "h6":
          case "header":
          case "hgroup":
          case "hr":
          case "i":
          case "iframe":
          case "img":
          case "input":
          case "ins":
          case "kbd":
          case "keygen":
          case "label":
          case "map":
          case "mark":
          case "math":
          case "menu":
          case "meter":
          case "nav":
          case "noscript":
          case "object":
          case "ol":
          case "output":
          case "p":
          case "pre":
          case "progress":
          case "q":
          case "ruby":
          case "s":
          case "samp":
          case "script":
          case "section":
          case "select":
          case "small":
          case "span":
          case "strong":
          case "sub":
          case "sup":
          case "svg":
          case "table":
          case "textarea":
          case "time":
          case "ul":
          case "var":
          case "video":
          case "wbr":
          case "text":


          case "area":
          //(if it is a descendant of a map element)

          case "style":
          //(if the scoped attribute is present)


          default:
            return false;
        }

      }
    }


  }
}
