using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Ivony.Html.Web
{
  public class HtmlMetaCollection : NameValueCollection
  {

    public HtmlHead Head
    {
      get;
      private set;
    }

    public HtmlMetaCollection( HtmlHead head )
    {
      Head = head;
    }

    public override void Add( string name, string value )
    {


    }


    private class HtmlMeta
    {

      public static HtmlMeta Create( HtmlHead head, string name, string content )
      {
        var factory = head.Element.Document.GetNodeFactory();
        var freeElement = factory.CreateElement( "meta" );

        var element = head.Element.Insert( GetMetaIndex( head ), freeElement );

        element.SetAttribute( "name", name );
        element.SetAttribute( "content", content );

        return new HtmlMeta( element );
      }

      private HtmlMeta( IHtmlElement element )
      {
        Element = element;
      }


      private static int GetMetaIndex( HtmlHead head )
      {

        var lastMeta = head.Element.Elements( "meta" ).LastOrDefault();
        if ( lastMeta != null )
          return lastMeta.IndexOfSelf() + 1;

        var titleElement = head.Element.Elements( "title" ).SingleOrDefault();
        if ( titleElement != null )
          return titleElement.IndexOfSelf() + 1;

        return 0;

      }

      public IHtmlElement Element
      {
        get;
        private set;
      }
    }

  }
}
