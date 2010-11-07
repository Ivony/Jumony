using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
  public static class DocumentExtensions
  {

    /// <summary>
    /// 在文档中通过ID来查找元素
    /// </summary>
    /// <param name="document">要查找元素的文档</param>
    /// <param name="id">元素ID</param>
    /// <returns>找到的元素，没有符合要求的则返回null</returns>
    /// <exception cref="System.InvalidOperationException">找到多个ID相同的元素</exception>
    public static IHtmlElement GetElementById( this IHtmlDocument document, string id )
    {
      return document.Descendants().SingleOrDefault( element => element.Attribute( "id" ).Value() == id );
    }


    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element )
    {
      return Identity( element, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create )
    {
      return Identity( element, create, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <param name="ancestorsCreate">在创建ID的过程中，是否为没有唯一ID的父级也创建ID</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create, bool ancestorsCreate )
    {
      EnsureAllocated( element );

      var id = element.Attribute( "id" ).Value();

      if ( string.IsNullOrEmpty( id ) || !element.Document.Descendants().Where( e => e.Attribute( "id" ).Value() == id ).OnlyOne() )
        id = null;

      if ( create && id == null )
        element.SetAttribute( "id" ).Value( id = CreateIdentity( element, ancestorsCreate ) );


      return id;
    }

    private static string CreateIdentity( IHtmlElement element, bool ancestorsCreate )
    {
      string parentId;

      var parentElement = element.Parent as IHtmlElement;
      if ( parentElement != null )
      {
        parentId = Identity( parentElement );
        if ( parentId == null )
        {
          if ( ancestorsCreate )
            parentId = Identity( parentElement, true, true );
          else
            parentId = CreateIdentity( parentElement, false );
        }
      }
      else
      {
        if ( element.Parent is IHtmlDocument )
          parentId = null;
        else
          throw new InvalidOperationException();
      }

      var name = GetElementName( element );

      var builder = new StringBuilder();

      if ( parentId != null )
        builder.AppendFormat( "{0}_", parentId );

      builder.Append( name );

      if ( element.Siblings().Where( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) ).OnlyOne() )
        return builder.ToString();



      var index = element.SiblingsBeforeSelf().Count( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) );

      builder.Append( index + 1 );

      return builder.ToString();
    }


    private static void EnsureAllocated( IHtmlNode node )
    {
      while ( true )
      {
        if ( node is IFreeNode || node is HtmlFragment )
          throw new InvalidOperationException( "无法对没有被分配在文档上的元素或节点进行操作" );

        node = node.Parent;
        if ( node == null )
          break;
      }
    }

    private static string GetElementName( IHtmlElement element )
    {

      switch ( element.Name.ToLowerInvariant() )
      {
        case "a":
          if ( element.Attribute( "name" ) != null )
            return "link";
          else
            return "anchor";

        case "li":
          return "item";

        case "ul":
        case "ol":
        case "dl":
          return "list";

        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
          return "header";

        default:
          return element.Name;

      }
    }
  }
}
