using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Parser
{
  public class DomTextNode : DomNode, IHtmlTextNode
  {

    private readonly string raw;

    public DomTextNode( string rawHtml )
    {
      raw = rawHtml;
    }


    protected override string ObjectName
    {
      get { return "TextNode"; }
    }



    public string HtmlText
    {
      get
      {
        var element =  this.Parent();
        if ( element != null )
        {
          if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
            return raw;

          else
            return HtmlEncoding.HtmlEncode( HtmlEncoding.HtmlDecode( raw ) );
        }

        return raw;
      }
    }


    public override string RawHtml
    {
      get
      {
        return raw;
      }
    }

  }




}
