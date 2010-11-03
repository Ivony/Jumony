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
    /// 标识一个元素，如果这个元素有唯一ID属性，则返回，否则便为其创建
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identify( this IHtmlElement element )
    {
      EnsureAllocated( element );

      var id = ID( element );

      if ( id == null )
        element.SetAttribute( "id" ).Value( id = CreateIdentity( element ) );

      return id;
    }

    public static string ID( this IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();

      if ( string.IsNullOrEmpty( id ) || element.Document.Descendants().Count( e => e.Attribute( "id" ).Value() == id ) > 1 )
        return null;

      return id;
    }

    private static string CreateIdentity( IHtmlElement element )
    {
      EnsureAllocated( element );

      string parentId;

      var parentElement = element.Parent as IHtmlElement;
      if ( parentElement != null )
      {
        parentId = ID( parentElement );
        if ( parentId == null )
          parentId = CreateIdentity( parentElement );
      }
      else
      {
        if ( element.Parent is IHtmlDocument )
          parentId = null;
        else
          throw new InvalidOperationException();
      }

      var name = GetElementName( element );
      var index = element.SiblingsBeforeSelf().Count( e => GetElementName( e ) == GetElementName( element ) );

      index += 1;

      if ( parentId == null )
        return string.Format( "{0}{1}", name, index );
      else
        return string.Format( "{0}_{1}{2}", parentId, name, index );

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
