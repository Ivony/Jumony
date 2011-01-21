using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ivony.Html.Web
{
  public static class WebExtenions
  {

    public static HtmlHead Head( this IHtmlDocument document )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );

      var htmlElement = document.Elements( "html" ).SingleOrDefault();

      if ( htmlElement == null )
        return null;

      var headElement = htmlElement.Elements( "head" ).SingleOrDefault();
      if ( headElement == null )
      {
        var freeHead = htmlElement.Document.GetNodeFactory().CreateElement( "head" );
        headElement = freeHead.InsertTo( htmlElement, 0 );
      }

      return new HtmlHead( headElement );
    }

  }
}
