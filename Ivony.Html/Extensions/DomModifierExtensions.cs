using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public static class DomModifierExtensions
  {

    /// <summary>
    /// 在容器末尾增加一个元素
    /// </summary>
    /// <param name="modifier">DOM 结构修改器</param>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="elementName">元素名</param>
    /// <returns>添加的元素</returns>
    public static IHtmlElement AddElement( this IHtmlDomModifier modifier, IHtmlContainer container, string elementName )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );

      lock ( container.SyncRoot )
      {
        return modifier.AddElement( container, container.Nodes().Count(), elementName );
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
