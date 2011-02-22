using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// 提供修改文档对象模型（DOM）的扩展方法
  /// </summary>
  public static class DomExtensions
  {

    /// <summary>
    /// 创建文档碎片
    /// </summary>
    /// <param name="document">要创建文档碎片的文档</param>
    /// <returns>创建的文档碎片</returns>
    public static IHtmlFragment CreateFragment( this IHtmlDocument document )
    {
      var manager = document.FragmentManager;
      if ( manager == null )
        throw new NotSupportedException();

      return manager.CreateFragment();
    }

    /// <summary>
    /// 使用指定文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="text">文本内容</param>
    public static void InnerText( this IHtmlElement element, string text )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      var manager = element.Document.FragmentManager;

      if ( manager == null )
        throw new NotSupportedException();

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( !HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          var fragment = manager.CreateFragment();

          fragment.AddTextNode( text );

          fragment.Into( element, 0 );
        }
        else
        {
          var fragment = HtmlEncode( text, manager );

          fragment.Into( element, 0 );
        }
      }

    }


    private static IHtmlFragment HtmlEncode( string text, IHtmlFragmentManager manager )
    {

      if ( text == null )
        throw new ArgumentNullException( "text" );

      if ( manager == null )
        throw new ArgumentNullException( "manager" );


      var fragment = manager.CreateFragment();
      var encoded = HtmlEncoding.HtmlEncode( text );

      encoded = encoded.Replace( "\r\n", "\n" ).Replace( "\r", "\n" );

      int index = 0, brIndex = 0;
      while ( true )
      {
        brIndex = encoded.IndexOf( '\n', index );

        if ( brIndex == -1 )
        {
          if ( index < encoded.Length )
            fragment.AddTextNode( encoded.Substring( index ) );

          break;
        }



        if ( index != brIndex )
          fragment.AddTextNode( encoded.Substring( index, brIndex - index ) );
        fragment.AddElement( "br", new Dictionary<string, string>() );
        index = brIndex + 1;

      }

      return fragment;
    }


    /// <summary>
    /// 解析 HTML 并创建文档碎片
    /// </summary>
    /// <param name="document">要创建文档碎片的文档</param>
    /// <param name="html">要解析的 HTML</param>
    /// <returns>创建的文档碎片</returns>
    public static IHtmlFragment ParseFragment( this IHtmlDocument document, string html )
    {
      var fragmentManager = document.FragmentManager;

      if ( fragmentManager == null )
        throw new NotSupportedException();

      return fragmentManager.ParseFragment( html );
    }

    /// <summary>
    /// 使用指定的HTML文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="html">要替换的HTML代码</param>
    public static void InnerHtml( this IHtmlElement element, string html )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );


      var manager = element.Document.FragmentManager;

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          var fragment = element.Document.CreateFragment();

          fragment.AddTextNode( html );

          fragment.Into( element, 0 );
        }
        else
        {
          var fragment = element.Document.ParseFragment( html );

          fragment.Into( element, 0 );
        }
      }
    }



    /// <summary>
    /// 清除所有子节点
    /// </summary>
    /// <param name="element">要清除所有子节点的元素</param>
    public static IHtmlElement ClearNodes( this IHtmlElement element )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      lock ( element.SyncRoot )
      {
        var childs = element.Nodes().ToArray();//清除所有的子节点
        foreach ( var node in childs )
          node.Remove();
      }

      return element;
    }




    public static IHtmlElement InsertElement( this IHtmlContainer container, int index, string name )
    {
      var fragment = container.Document.CreateFragment();

      fragment.AddElement( name, new Dictionary<string, string>() );

      return (IHtmlElement) fragment.Into( container, index ).Single();
    }


    public static IHtmlTextNode InsertTextNode( this IHtmlContainer container, int index, string htmlText )
    {
      var fragment = container.Document.CreateFragment();

      fragment.AddTextNode( htmlText );

      return (IHtmlTextNode) fragment.Into( container, index ).Single();
    }


    public static IHtmlComment InsertComment( this IHtmlContainer container, int index, string comment )
    {
      var fragment = container.Document.CreateFragment();

      fragment.AddComment( comment );

      return (IHtmlComment) fragment.Into( container, index ).Single();
    }



  }
}
