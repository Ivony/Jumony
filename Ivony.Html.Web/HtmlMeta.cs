using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  public class HtmlMeta
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


    public HtmlMeta( IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "meta" ) )
        throw new InvalidOperationException();

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


    public string Name
    {
      get { return Element.Attribute( "name" ).Value(); }
    }

    public string Content
    {
      get { return Element.Attribute( "content" ).Value(); }
      set { Element.SetAttribute( "content", value ); }
    }


    public IHtmlElement Element
    {
      get;
      private set;
    }
  }
}
