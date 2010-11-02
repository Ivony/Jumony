using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
  public static class DocumentExtensions
  {

    public static IHtmlElement GetElementById( this IHtmlDocument document, string id )
    {
      return document.Descendants().SingleOrDefault( element => element.Attribute( "id" ).Value() == id );
    }

    public static string Identify( this IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();

      if ( string.IsNullOrEmpty( id ) || element.Document.Descendants().Count( e => e.Attribute( "id" ).Value() == id ) > 1 )
        element.SetAttribute( "id" ).Value( id = CreateIdentity( element ) );

      return id;
    }

    private static string CreateIdentity( IHtmlElement element )
    {
      return Guid.NewGuid().ToString();
    }

  }
}
