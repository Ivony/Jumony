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



    private static IHtmlDomModifier EnsureModifiable( IHtmlDomObject domObject )
    {
      var modifier = domObject.Document.DomModifier;
      if ( modifier == null )
        throw new NotSupportedException( "文档不支持修改 DOM 结构" );

      return modifier;
    }




    /// <summary>
    /// 尝试为容器添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="name">元素名</param>
    /// <returns>创建并添加好的元素</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlElement AddElement( this IHtmlContainer container, string name )
    {
      lock ( container.SyncRoot )
      {
        return AddElement( container, container.Nodes().Count(), name );
      }
    }

    /// <summary>
    /// 尝试为容器添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="name">元素名</param>
    /// <returns>创建并添加好的元素</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlElement AddElement( this IHtmlContainer container, int index, string name )
    {
      var modifier = EnsureModifiable( container );

      return modifier.AddElement( container, index, name );
    }


    /// <summary>
    /// 尝试为容器添加一个文本节点
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>创建并添加好的文本节点</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlTextNode AddTextNode( this IHtmlContainer container, string htmlText )
    {
      lock ( container.SyncRoot )
      {
        return AddTextNode( container, container.Nodes().Count(), htmlText );
      }
    }

    /// <summary>
    /// 尝试为容器添加一个文本节点
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>创建并添加好的文本节点</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlTextNode AddTextNode( this IHtmlContainer container, int index, string htmlText )
    {
      var modifier = EnsureModifiable( container );

      return modifier.AddTextNode( container, index, htmlText );
    }


    /// <summary>
    /// 尝试为容器添加一个注释节点
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="comment">HTML 注释文本</param>
    /// <returns>创建并添加好的注释节点</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlComment AddComment( this IHtmlContainer container, string comment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      lock ( container.SyncRoot )
      {
        return AddComment( container, container.Nodes().Count(), comment );
      }
    }

    /// <summary>
    /// 尝试为容器添加一个注释节点
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="comment">HTML 注释文本</param>
    /// <returns>创建并添加好的注释节点</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlComment AddComment( this IHtmlContainer container, int index, string comment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );



      var modifier = EnsureModifiable( container );

      return modifier.AddComment( container, index, comment );
    }



    /// <summary>
    /// 尝试从 DOM 中移除此节点
    /// </summary>
    /// <param name="node">要被移除的节点</param>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static void Remove( this IHtmlNode node )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );


      var modifier = EnsureModifiable( node );

      modifier.RemoveNode( node );
    }





    /// <summary>
    /// 尝试使用指定的 HTML 碎片替换此节点
    /// </summary>
    /// <param name="node">要被替换的节点</param>
    /// <param name="fragment">替换节点的 HTML 碎片</param>
    /// <returns>碎片置入后产生的节点集</returns>
    public static IEnumerable<IHtmlNode> ReplaceWith( this IHtmlNode node, IHtmlFragment fragment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );

      if ( !object.Equals( fragment.Document, node.Document ) )
        throw new InvalidOperationException();





      lock ( node.Container )
      {
        int index = node.NodesIndexOfSelf();
        Remove( node );
        return fragment.Into( node.Container, index );
      }
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

      var modifier = EnsureModifiable( element );

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( !HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          modifier.AddTextNode( element, 0, text );
        }
        else
        {
          AddEncodedText( text, element, modifier );
        }
      }

    }


    private static void AddEncodedText( string text, IHtmlContainer container, IHtmlDomModifier modifier )
    {

      if ( text == null )
        throw new ArgumentNullException( "text" );

      var encoded = HtmlEncoding.HtmlEncode( text );

      encoded = encoded.Replace( "\r\n", "\n" ).Replace( "\r", "\n" );

      int index = 0, brIndex = 0;
      while ( true )
      {
        brIndex = encoded.IndexOf( '\n', index );

        if ( brIndex == -1 )
        {
          if ( index < encoded.Length )
            modifier.AddTextNode( container, encoded.Substring( index ) );

          break;
        }



        if ( index != brIndex )
          modifier.AddTextNode( container, encoded.Substring( index, brIndex - index ) );

        modifier.AddElement( container, "br" );
        index = brIndex + 1;

      }
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


      var modifier = EnsureModifiable( element );

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          modifier.AddTextNode( element, html );
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




  }
}
