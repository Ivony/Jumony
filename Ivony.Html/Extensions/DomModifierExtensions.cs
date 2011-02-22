using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public static class DomModifierExtensions
  {

    public static IHtmlElement AddElement( this IHtmlDomModifier modifier, IHtmlContainer container, string name )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( name == null )
        throw new ArgumentNullException( "name" );

      lock ( container.SyncRoot )
      {
        return modifier.AddElement( container, container.Nodes().Count(), name );
      }
    }



    public static IHtmlTextNode AddTextNode( this IHtmlDomModifier modifier, IHtmlContainer container, string htmlText )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      lock ( container.SyncRoot )
      {
        return modifier.AddTextNode( container, container.Nodes().Count(), htmlText );
      }
    }



    public static IHtmlComment AddComment( this IHtmlDomModifier modifier, IHtmlContainer container, string comment )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "htmlText" );

      lock ( container.SyncRoot )
      {
        return modifier.AddComment( container, container.Nodes().Count(), comment );
      }
    }

  }
}
