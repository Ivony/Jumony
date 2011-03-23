using System;
using System.Collections.Generic;
using System.Linq;

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
      if ( document == null )
        throw new ArgumentNullException( "document" );


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
      if ( document == null )
        throw new ArgumentNullException( "document" );

      if ( html == null )
        throw new ArgumentNullException( "html" );



      var manager = document.FragmentManager;
      if ( manager == null )
        throw new NotSupportedException();

      return manager.ParseFragment( html );
    }



    private static IHtmlDomModifier EnsureModifiable( IHtmlDomObject domObject )
    {
      if ( domObject == null )
        throw new ArgumentNullException( "domObject" );


      var modifier = domObject.Document.DomModifier;
      if ( modifier == null )
        throw new NotSupportedException( "文档不支持修改 DOM 结构" );

      return modifier;
    }




    /// <summary>
    /// 尝试为容器添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="elementName">元素名</param>
    /// <returns>创建并添加好的元素</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlElement AddElement( this IHtmlContainer container, string elementName )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );



      lock ( container.SyncRoot )
      {
        return AddElement( container, container.Nodes().Count(), elementName );
      }
    }

    /// <summary>
    /// 尝试为容器添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="elementName">元素名</param>
    /// <returns>创建并添加好的元素</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlElement AddElement( this IHtmlContainer container, int index, string elementName )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );



      var modifier = EnsureModifiable( container );

      return modifier.AddElement( container, index, elementName );
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
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );


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
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );



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
    /// 尝试为容器添加一个文档碎片
    /// </summary>
    /// <param name="container">要添加碎片的容器</param>
    /// <param name="fragment">要添加的碎片</param>
    /// <returns></returns>
    public static IEnumerable<IHtmlNode> AddFragment( this IHtmlContainer container, IHtmlFragment fragment )
    {

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );



      lock ( container.SyncRoot )
      {
        return AddFragment( container, container.Nodes().Count(), fragment );
      }
    }


    /// <summary>
    /// 尝试为容器添加一个文档碎片
    /// </summary>
    /// <param name="container">要添加碎片的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="fragment">要添加的碎片</param>
    /// <returns></returns>
    public static IEnumerable<IHtmlNode> AddFragment( this IHtmlContainer container, int index, IHtmlFragment fragment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );


      return fragment.Into( container, index );

    }


    /// <summary>
    /// 尝试为容器添加一个文档碎片
    /// </summary>
    /// <param name="container">要添加碎片的容器</param>
    /// <param name="html">要分析成碎片的 HTML 文本</param>
    /// <returns></returns>
    public static IEnumerable<IHtmlNode> AddFragment( this IHtmlContainer container, string html )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( html == null )
        throw new ArgumentNullException( "html" );


      var fragment = container.Document.ParseFragment( html );

      return AddFragment( container, fragment );

    }


    /// <summary>
    /// 尝试为容器添加一个文档碎片
    /// </summary>
    /// <param name="container">要添加碎片的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="html">要分析成碎片的 HTML 文本</param>
    /// <returns></returns>
    public static IEnumerable<IHtmlNode> AddFragment( this IHtmlContainer container, int index, string html )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( html == null )
        throw new ArgumentNullException( "html" );


      var fragment = container.Document.ParseFragment( html );

      return AddFragment( container, index, fragment );

    }





    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="textNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlTextNode AddCopy( this IHtmlContainer container, IHtmlTextNode textNode )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );


      lock ( container.SyncRoot )
      {
        return AddCopy( container, container.Nodes().Count(), textNode );
      }
    }


    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="textNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlTextNode AddCopy( this IHtmlContainer container, int index, IHtmlTextNode textNode )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );


      return container.AddTextNode( index, textNode.HtmlText );
    }




    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="comment">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlComment AddCopy( this IHtmlContainer container, IHtmlComment comment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      lock ( container.SyncRoot )
      {
        return AddCopy( container, container.Nodes().Count(), comment );
      }
    }


    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="comment">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlComment AddCopy( this IHtmlContainer container, int index, IHtmlComment comment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      return container.AddComment( index, comment.Comment );
    }



    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="element">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlElement AddCopy( this IHtmlContainer container, IHtmlElement element )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( element == null )
        throw new ArgumentNullException( "element" );


      lock ( container.SyncRoot )
      {
        return AddCopy( container, container.Nodes().Count(), element );
      }
    }


    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="element">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlElement AddCopy( this IHtmlContainer container, int index, IHtmlElement element )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( element == null )
        throw new ArgumentNullException( "element" );


      var _element = container.AddElement( index, element.Name );

      foreach ( var attribute in element.Attributes() )
        _element.AddAttribute( attribute.Name, attribute.AttributeValue );


      foreach ( var node in element.Nodes() )
        AddCopy( _element, node );


      return _element;
    }


    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="node">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlNode AddCopy( this IHtmlContainer container, IHtmlNode node )
    {
      lock ( container.SyncRoot )
      {
        return AddCopy( container, container.Nodes().Count(), node );
      }
    }


    /// <summary>
    /// 添加节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="node">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlNode AddCopy( this IHtmlContainer container, int index, IHtmlNode node )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( node == null )
        throw new ArgumentNullException( "node" );


      var textNode = node as IHtmlTextNode;
      if ( textNode != null )
        return AddCopy( container, index, textNode );

      var comment = node as IHtmlComment;
      if ( comment != null )
        return AddCopy( container, index, comment );

      var element = node as IHtmlElement;
      if ( element != null )
        return AddCopy( container, index, element );

      throw new NotSupportedException();
    }




    /// <summary>
    /// 创建碎片的副本
    /// </summary>
    /// <param name="fragment">要创建副本的碎片</param>
    /// <returns>碎片的副本</returns>
    public static IHtmlFragment MakeCopy( this IHtmlFragment fragment )
    {
      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );


      var _fragment = fragment.Document.CreateFragment();

      foreach ( var node in fragment.Nodes() )
        _fragment.AddCopy( node );


      return _fragment;
    }







    /// <summary>
    /// 添加一个属性
    /// </summary>
    /// <param name="element">要添加属性的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="attributeValue">属性值</param>
    /// <returns>添加的属性</returns>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    public static IHtmlAttribute AddAttribute( this IHtmlElement element, string attributeName, string attributeValue )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      var modifier = EnsureModifiable( element );

      return modifier.AddAttribute( element, attributeName, attributeValue );
    }



    /// <summary>
    /// 尝试从 DOM 中移除此节点
    /// </summary>
    /// <param name="node">要被移除的节点</param>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    /// <remarks>
    /// 若节点不存在（即为 null），则此方法不执行任何操作
    /// </remarks>
    public static void Remove( this IHtmlNode node )
    {
      if ( node == null )
        return;


      var modifier = EnsureModifiable( node );

      modifier.RemoveNode( node );
    }



    /// <summary>
    /// 尝试从 DOM 中移除指定节点
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    /// <param name="nodes">要移除的节点</param>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    /// <exception cref="System.InvalidOperationException">若节点不是位于同一文档</exception>
    public static void Remove<T>( this IEnumerable<T> nodes ) where T : IHtmlNode
    {

      if ( !nodes.Any() )
        return;

      var document = nodes.First().Document;
      var modifier = document.DomModifier;

      if ( modifier == null )
        throw new NotSupportedException();

      var array = nodes.ToArray();

      if ( array.Any( item => !item.Document.Equals( document ) ) )
        throw new InvalidOperationException();

      foreach ( var node in array )
      {
        modifier.RemoveNode( node );
      }
    }



    /// <summary>
    /// 尝试从 DOM 中移除此属性
    /// </summary>
    /// <param name="attribute">要被移除的属性</param>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    /// <remarks>
    /// 若属性不存在（即为 null），则此方法不执行任何操作
    /// </remarks>
    public static void Remove( this IHtmlAttribute attribute )
    {
      if ( attribute == null )
        return;

      if ( attribute.Element == null )
        throw new InvalidOperationException();


      var modifier = EnsureModifiable( attribute.Element );

      modifier.RemoveAttribute( attribute );
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

      var container = node.Container;

      lock ( container )
      {
        int index = node.NodesIndexOfSelf();
        Remove( node );
        return fragment.Into( container, index );
      }
    }



    public static IEnumerable<IHtmlNode> ReplaceWithHtml( this IHtmlNode node, string html )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( html == null )
        throw new ArgumentNullException( "html" );

      var fragment = node.Document.ParseFragment( html );

      return ReplaceWith( node, fragment );

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


        if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          modifier.AddTextNode( element, text );
        }
        else if ( HtmlSpecification.preformatedElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          modifier.AddTextNode( element, HtmlEncoding.HtmlEncode( text ) );
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

      encoded = encoded.Replace( " ", "&nbsp;" );

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
