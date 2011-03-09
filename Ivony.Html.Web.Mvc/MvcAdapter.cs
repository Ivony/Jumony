using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html.Web.Mvc
{
  public class MvcAdapter : IHtmlAdapter
  {

    public bool Render( IHtmlNode node, System.IO.TextWriter writer )
    {
      var element = node as IHtmlElement;

      if ( element != null )
      {
        if ( element.Name.EqualsIgnoreCase( "a" ) && element.Attribute( "action" ) != null )
        {
          return RenderActionLink( element );
        }


        if ( element.Name.EqualsIgnoreCase( "partial" ) )
        {
          return RenderPartial( element );
        }

      }


      return false;
    }

    public bool RenderActionLink( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    private bool RenderPartial( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

  }
}
