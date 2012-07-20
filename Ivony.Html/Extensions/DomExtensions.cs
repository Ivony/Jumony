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
        throw new NotSupportedException( "文档不支持文档碎片" );

      return manager.ParseFragment( html );
    }



    private static IHtmlDomModifier EnsureModifiable( IHtmlDomObject domObject )
    {
      if ( domObject == null )
        throw new ArgumentNullException( "domObject" );


      var document = domObject.Document;
      if ( document == null )
        throw new InvalidOperationException( "无法修改不在 DOM 上的节点" );

      var modifier = document.DomModifier;
      if ( modifier == null )
        throw new NotSupportedException( "文档不支持修改 DOM 结构" );

      return modifier;
    }




    /// <summary>
    /// 添加一个元素
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
    /// 添加一个元素
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
    /// 添加一个文本节点
    /// </summary>
    /// <param name="container">要添加文本节点的容器</param>
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
    /// 添加一个文本节点
    /// </summary>
    /// <param name="container">要添加文本节点的容器</param>
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
    /// 添加一个注释节点
    /// </summary>
    /// <param name="container">要添加注释节点的容器</param>
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
    /// 添加一个注释节点
    /// </summary>
    /// <param name="container">要添加注释节点的容器</param>
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
    /// 添加一个文档碎片
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
    /// 添加一个文档碎片
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
    /// 添加一个文档碎片
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
    /// 添加一个文档碎片
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
    /// 添加注释节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="comment">要创作副本的注释节点</param>
    /// <returns>添加后的注释节点</returns>
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
    /// 添加注释节点的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="comment">要创作副本的注释节点</param>
    /// <returns>添加后的注释节点</returns>
    public static IHtmlComment AddCopy( this IHtmlContainer container, int index, IHtmlComment comment )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      return container.AddComment( index, comment.Comment );
    }



    /// <summary>
    /// 添加元素的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="element">要创作副本的元素</param>
    /// <returns>添加后的元素</returns>
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
    /// 添加元素的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="element">要创作副本的元素</param>
    /// <returns>添加后的元素</returns>
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
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( node == null )
        throw new ArgumentNullException( "node" );



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
    /// 添加节点集合的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="node">要创作副本的节点集合</param>
    /// <returns>添加后的节点集合</returns>
    public static IEnumerable<IHtmlNode> AddCopy( this IHtmlContainer container, IEnumerable<IHtmlNode> node )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( node == null )
        throw new ArgumentNullException( "node" );



      lock ( container.SyncRoot )
      {
        return AddCopy( container, container.Nodes().Count(), node );
      }
    }

    /// <summary>
    /// 添加节点集合的副本
    /// </summary>
    /// <param name="container">要添加副本的容器</param>
    /// <param name="index">要添加的位置</param>
    /// <param name="nodes">要创作副本的节点集合</param>
    /// <returns>添加后的节点集合</returns>
    public static IEnumerable<IHtmlNode> AddCopy( this IHtmlContainer container, int index, IEnumerable<IHtmlNode> nodes )
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( nodes == null )
        throw new ArgumentNullException( "nodes" );


      List<IHtmlNode> result = new List<IHtmlNode>( nodes.Count() );


      lock ( container.SyncRoot )
      {

        foreach ( var node in nodes.Reverse() )
        {
          var textNode = node as IHtmlTextNode;
          if ( textNode != null )
          {
            result.Add( AddCopy( container, index, textNode ) );
            continue;
          }

          var comment = node as IHtmlComment;
          if ( comment != null )
          {
            result.Add( AddCopy( container, index, comment ) );
            continue;
          }

          var element = node as IHtmlElement;
          if ( element != null )
          {
            result.Add( AddCopy( container, index, element ) );
            continue;
          }

          throw new NotSupportedException();
        }

        return result.ToArray();
      }
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
    /// 创建文档的副本
    /// </summary>
    /// <param name="provider">用于创建副本文档的 IHtmlDomProvider 对象</param>
    /// <param name="document">要创建副本的文档</param>
    /// <returns>文档的副本</returns>
    public static IHtmlDocument MakeCopy( this IHtmlDomProvider provider, IHtmlDocument document )
    {
      var copy = provider.CreateDocument( document.DocumentUri );


      copy.AddCopy( provider, document.Nodes() );

      return copy;

    }


    private static IHtmlContainer AddCopy( this IHtmlContainer container, IHtmlDomProvider provider, IEnumerable<IHtmlNode> nodes )
    {
      foreach ( var node in nodes )
      {

        if ( node is IHtmlSpecial )
          throw new NotSupportedException();


        var element = node as IHtmlElement;

        if ( element != null )
        {
          var attributes = element.Attributes() as IDictionary<string, string>;
          if ( attributes == null )
            attributes = element.Attributes().ToDictionary( a => a.Name, a => a.AttributeValue );

          var copy = provider.AddElement( container, element.Name, attributes );

          copy.AddCopy( provider, element.Nodes() );

          continue;

        }

        var textNode = node as IHtmlTextNode;

        if ( textNode != null )
        {
          provider.AddTextNode( container, textNode.RawHtml ?? textNode.HtmlText );

          continue;
        }

        var comment = node as IHtmlComment;

        if ( comment != null )
        {
          provider.AddComment( container, comment.Comment );

          continue;
        }


        var special = node as IHtmlSpecial;
        if ( special != null )
        {
          var html = special.RawHtml;
          if ( html == null )
            throw new InvalidOperationException();

          provider.AddSpecial( container, html );

          continue;
        }

        throw new NotSupportedException();







      }

      return container;
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
    /// 移除指定的属性
    /// </summary>
    /// <param name="element">要移除属性的元素</param>
    /// <param name="attributeName">要移除的属性名称</param>
    public static T RemoveAttribute<T>( this T element, string attributeName ) where T : IHtmlElement
    {
      lock ( element.SyncRoot )
      {

        var attribute = element.Attribute( attributeName );

        if ( attribute != null )
          attribute.Remove();

        return element;
      }
    }



    /// <summary>
    /// 尝试从 DOM 中移除这些节点
    /// </summary>
    /// <param name="nodes">要移除的节点</param>
    /// <exception cref="System.NotSupportedException">若文档不支持修改 DOM 结构</exception>
    /// <exception cref="System.InvalidOperationException">若节点不是位于同一文档</exception>
    public static void Remove( this IEnumerable<IHtmlNode> nodes )
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
    /// <exception cref="System.InvalidOperationException">若节点和碎片不在同一文档</exception>
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


    /// <summary>
    /// 尝试使用指定的 HTML 文本片段替换此节点
    /// </summary>
    /// <param name="node">要被替换的节点</param>
    /// <param name="html">替换节点的 HTML 文本</param>
    /// <returns>HTML 文本置入后产生的节点集</returns>
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
    public static T InnerText<T>( this T element, string text ) where T : IHtmlElement
    {
      return InnerText( element, text, false );
    }


    /// <summary>
    /// 使用指定文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="text">文本内容</param>
    /// <param name="encodeWhiteSpaces">是否编码空白字符</param>
    public static T InnerText<T>( this T element, string text, bool encodeWhiteSpaces ) where T : IHtmlElement
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      var modifier = EnsureModifiable( element );

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( string.IsNullOrEmpty( text ) )//对于空输入，则只需要清空元素即可
          return element;


        if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {

          modifier.AddTextNode( element, text );
        }
        else if ( HtmlSpecification.preformatedElements.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) || !encodeWhiteSpaces )
        {
          modifier.AddTextNode( element, HtmlEncoding.HtmlEncode( text ) );
        }
        else
        {
          var encoded = HtmlEncoding.HtmlEncode( text );

          encoded = encoded.Replace( "  ", " &nbsp;" );

          encoded = encoded.Replace( "\r\n", "\n" ).Replace( "\r", "\n" );

          encoded = encoded.Replace( "\n", "<br />" );


          element.Document.ParseFragment( encoded ).Into( element, 0 );
        }
      }

      return element;

    }



    /// <summary>
    /// 使用指定的HTML文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="html">要替换的HTML代码</param>
    public static T InnerHtml<T>( this T element, string html ) where T : IHtmlElement
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );


      var modifier = EnsureModifiable( element );

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( string.IsNullOrEmpty( html ) )//对于空输入，则只需要清空元素即可
          return element;


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

      return element;
    }



    /// <summary>
    /// 清除所有子节点
    /// </summary>
    /// <param name="container">要清除所有子节点的容器</param>
    public static T ClearNodes<T>( this T container ) where T : IHtmlContainer
    {

      if ( container == null )
        throw new ArgumentNullException( "container" );

      lock ( container.SyncRoot )
      {
        var childs = container.Nodes().ToArray();//清除所有的子节点
        foreach ( var node in childs )
          node.Remove();
      }

      return container;
    }




  }
}
