using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class HtmlHead
  {

    private IHtmlElement _element;

    public HtmlHead( IHtmlElement element )
    {
      _element = element;
    }


    public IHtmlElement Element
    {
      get { return _element; }
    }

    public string Title
    {
      get
      {
        var titleElement = Element.Elements( "title" ).SingleOrDefault();
        if ( titleElement == null )
          return null;

        return titleElement.InnerText();
      }
      set
      {
        var titleElement = Element.Elements( "title" ).SingleOrDefault();

        if ( titleElement == null )
        {
          var factory = Element.Document.GetNodeFactory();
          titleElement = (IHtmlElement) factory.CreateElement( "title" ).InsertTo( Element, 0 );
        }

        titleElement.InnerText( value );
      }
    }

  }
}
