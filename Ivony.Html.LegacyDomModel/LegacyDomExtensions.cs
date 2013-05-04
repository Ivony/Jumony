using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{


  /// <summary>
  /// 提供修改文档对象模型（DOM）的扩展方法
  /// </summary>
  [Obsolete( "该部分 API 已经过时" )]
  public static class LegacyDomExtensions
  {

    /// <summary>
    /// 在末尾添加节点
    /// </summary>
    /// <param name="container">被添加节点的容器</param>
    /// <param name="node">要添加的节点</param>
    /// <returns>被添加节点的容器</returns>
    /// <exception cref="System.InvalidOperationException">添加的节点与节点容器不属于同一个文档</exception>
    public static TContainer Append<TContainer>( this TContainer container, IFreeNode node ) where TContainer : IHtmlContainer
    {
      return Insert( container, container.Nodes().Count(), node );
    }


    /// <summary>
    /// 在指定位置添加节点
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="index">要插入节点的位置</param>
    /// <param name="node">要添加的节点</param>
    /// <exception cref="System.InvalidOperationException">添加的节点与节点容器不属于同一个文档</exception>
    public static TContainer Insert<TContainer>( this TContainer container, int index, IFreeNode node ) where TContainer : IHtmlContainer
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( node == null )
        throw new ArgumentNullException( "node" );


      var fragment = container as HtmlFragment;

      if ( fragment != null )
      {
        fragment.AddNode( index, node );
        return container;
      }



      lock ( container.SyncRoot )
      {
        if ( !container.Document.Equals( node.Document ) )
          throw new InvalidOperationException();

        node.Into( container, index );

        return container;
      }
    }

    /// <summary>
    /// 将元素插入到指定位置
    /// </summary>
    /// <param name="element">要插入的游离元素</param>
    /// <param name="container">要被插入的容器</param>
    /// <param name="index">插入位置</param>
    /// <returns>插入后的元素</returns>
    public static IHtmlElement InsertTo( this IFreeElement element, IHtmlContainer container, int index )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      lock ( container.SyncRoot )
      {
        if ( !container.Document.Equals( element.Document ) )
          throw new InvalidOperationException();

        return (IHtmlElement) element.Into( container, index );
      }
    }

    /// <summary>
    /// 将元素添加到容器末尾
    /// </summary>
    /// <param name="element">要插入的游离元素</param>
    /// <param name="container">要被插入的容器</param>
    /// <returns>添加后的元素</returns>
    public static IHtmlElement AppendTo( this IFreeElement element, IHtmlContainer container )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      return element.InsertTo( container, container.Nodes().Count() );
    }


    /// <summary>
    /// 将文本节点插入到指定位置。
    /// </summary>
    /// <param name="textNode">要插入的文本节点</param>
    /// <param name="container">被插入的容器</param>
    /// <param name="index">插入的位置</param>
    /// <returns>插入后的节点</returns>
    public static IHtmlTextNode InsertTo( this IFreeTextNode textNode, IHtmlContainer container, int index )
    {
      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      lock ( container.SyncRoot )
      {
        if ( !container.Document.Equals( textNode.Document ) )
          throw new InvalidOperationException();

        return (IHtmlTextNode) textNode.Into( container, index );
      }
    }

    /// <summary>
    /// 将文本节点添加到指定容器的末尾。
    /// </summary>
    /// <param name="textNode">要添加的文本节点</param>
    /// <param name="container">被添加的容器</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlTextNode AppendTo( this IFreeTextNode textNode, IHtmlContainer container )
    {
      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      return textNode.InsertTo( container, container.Nodes().Count() );
    }


    /// <summary>
    /// 将 HTML 注释插入到指定位置。
    /// </summary>
    /// <param name="comment">要插入的 HTML 注释</param>
    /// <param name="container">被插入的容器</param>
    /// <param name="index">插入的位置</param>
    /// <returns>插入后的 HTML 注释</returns>
    public static IHtmlComment InsertTo( this IFreeComment comment, IHtmlContainer container, int index )
    {
      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      lock ( container.SyncRoot )
      {
        if ( !container.Document.Equals( comment.Document ) )
          throw new InvalidOperationException();

        return (IHtmlComment) comment.Into( container, index );
      }
    }

    /// <summary>
    /// 将 HTML 注释添加到指定容器的末尾。
    /// </summary>
    /// <param name="comment">要添加的 HTML 注释</param>
    /// <param name="container">被添加的容器</param>
    /// <returns>添加后的 HTML 注释</returns>
    public static IHtmlComment AppendTo( this IFreeComment comment, IHtmlContainer container )
    {
      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      if ( container == null )
        throw new ArgumentNullException( "container" );


      return comment.InsertTo( container, container.Nodes().Count() );
    }




    /// <summary>
    /// 替换指定节点
    /// </summary>
    /// <param name="oldNode">被替换的节点</param>
    /// <param name="newNode">用于替换的节点</param>
    /// <exception cref="System.InvalidOperationException">被替换的节点与用于替换的节点不属于同一个文档</exception>
    public static IHtmlNode Replace( this IHtmlNode oldNode, IFreeNode newNode )
    {
      if ( oldNode == null )
        throw new ArgumentNullException( "oldNode" );

      if ( newNode == null )
        throw new ArgumentNullException( "newNode" );


      var container = oldNode.Container;

      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        var result = newNode.Into( container, oldNode.NodesIndexOfSelf() );
        oldNode.Remove();

        return result;
      }
    }

    /// <summary>
    /// 替换指定节点
    /// </summary>
    /// <param name="oldNode">被替换的节点</param>
    /// <param name="fragment">用于替换的碎片</param>
    /// <exception cref="System.InvalidOperationException">被替换的节点与用于替换的节点不属于同一个文档</exception>
    public static IEnumerable<IHtmlNode> Replace( this IHtmlNode oldNode, HtmlFragment fragment )
    {
      if ( oldNode == null )
        throw new ArgumentNullException( "oldNode" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );


      var container = oldNode.Container;

      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        var result = fragment.InsertTo( container, oldNode.NodesIndexOfSelf() );
        oldNode.Remove();

        return result;
      }
    }


    /*
    /// <summary>
    /// 在末尾添加节点的副本，此方法总是创建节点的副本
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="node">要添加的节点</param>
    /// <remarks>
    /// 此方法要求容器所在的文档支持创建节点，即GetNodeFactory方法不能返回null，否则将抛出NotSupportedException。
    /// 使用此方法可以轻易地将元素或者其他节点从一个文档复制到另一个文档。
    /// </remarks>
    /// <exception cref="System.NotSupportedException">容器所在的文档不支持创建节点，或不支持创建此类节点的副本（譬如说IHtmlDocument）</exception>
    public static TContainer AppendCopy<TContainer>( this TContainer container, IHtmlNode node ) where TContainer : IHtmlContainer
    {
      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( node == null )
        throw new ArgumentNullException( "node" );


      lock ( container.SyncRoot )
      {
        return InsertCopy( container, container.Nodes().Count(), node );
      }
    }


    /// <summary>
    /// 在指定位置添加节点的副本，此方法总是创建节点的副本
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="index">要插入节点的位置</param>
    /// <param name="node">要添加的节点</param>
    /// <remarks>
    /// 此方法要求容器所在的文档支持创建节点，即GetNodeFactory方法不能返回null，否则将抛出NotSupportedException。
    /// 使用此方法可以轻易地将元素或者其他节点从一个文档复制到另一个文档。
    /// </remarks>
    /// <exception cref="System.NotSupportedException">容器所在的文档不支持创建节点，或不支持创建此类节点的副本（譬如说IHtmlDocument）</exception>
    public static TContainer InsertCopy<TContainer>( this TContainer container, int index, IHtmlNode node ) where TContainer : IHtmlContainer
    {
      var fragment = container as HtmlFragment;

      if ( fragment != null )
      {
        fragment.AddCopy( index, node );
        return container;
      }


      lock ( container.SyncRoot )
      {

        var factory = container.Document.GetNodeFactory();
        if ( factory == null )
          throw new NotSupportedException();

        var nodeCopy = factory.MakeCopy( node );

        nodeCopy.Into( container, index );

        return container;
      }
    }
    */


    /*
    /// <summary>
    /// 替换指定节点
    /// </summary>
    /// <param name="oldNode">被替换的节点</param>
    /// <param name="newNode">用于替换的节点</param>
    /// <remarks>
    /// 此方法要求容器所在的文档支持创建节点，即GetNodeFactory方法不能返回null，否则将抛出NotSupportedException。
    /// 使用此方法可以轻易地将元素或者其他节点从一个文档复制到另一个文档。
    /// </remarks>
    /// <exception cref="System.NotSupportedException">容器所在的文档不支持创建节点，或不支持创建此类节点的副本（譬如说IHtmlDocument）</exception>
    public static IHtmlNode ReplaceCopy( this IHtmlNode oldNode, IHtmlNode newNode )
    {
      var container = oldNode.Container;

      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        var factory = container.Document.GetNodeFactory();
        if ( factory == null )
          throw new NotSupportedException();

        var nodeCopy = factory.MakeCopy( newNode );

        var result = nodeCopy.Into( container, oldNode.NodesIndexOfSelf() );
        oldNode.Remove();

        return result;
      }
    }
    */


    /// <summary>
    /// 创建节点的副本
    /// </summary>
    /// <param name="factory">用于创建节点的构建器</param>
    /// <param name="node">需要被创建副本的节点</param>
    /// <returns>节点的未分配副本</returns>
    /// <exception cref="System.NotSupportedException">试图创建不被支持的节点的副本</exception>
    public static IFreeNode MakeCopy( this IHtmlNodeFactory factory, IHtmlNode node )
    {

      if ( factory == null )
        throw new ArgumentNullException( "factory" );

      if ( node == null )
        throw new ArgumentNullException( "node" );


      var element = node as IHtmlElement;
      if ( element != null )
        return MakeCopy( factory, element );

      var comment = node as IHtmlComment;
      if ( comment != null )
        return MakeCopy( factory, comment );

      var textNode = node as IHtmlTextNode;
      if ( textNode != null )
        return MakeCopy( factory, textNode );

      throw new NotSupportedException();
    }


    /// <summary>
    /// 创建元素的副本
    /// </summary>
    /// <param name="factory">用于创建元素的构建器</param>
    /// <param name="element">需要被创建副本的元素</param>
    /// <returns>元素的未分配副本</returns>
    public static IFreeElement MakeCopy( this IHtmlNodeFactory factory, IHtmlElement element )
    {

      if ( factory == null )
        throw new ArgumentNullException( "factory" );

      if ( element == null )
        throw new ArgumentNullException( "element" );


      var free = factory.CreateElement( element.Name );
      foreach ( var attribute in element.Attributes() )
        free.AddAttribute( attribute.Name, attribute.Value() );

      CopyChildNodes( element, free );

      return free;
    }

    private static void CopyChildNodes( IHtmlElement element, IFreeElement freeElement )
    {

      foreach ( var node in element.Nodes().Reverse() )
      {
        var freeNode = freeElement.Factory.MakeCopy( node );

        freeNode.Into( freeElement, 0 );
      }

    }


    /// <summary>
    /// 创建注释的副本
    /// </summary>
    /// <param name="factory">用于创建注释的构建器</param>
    /// <param name="comment">需要被创建副本的注释</param>
    /// <returns>注释的未分配副本</returns>
    public static IFreeComment MakeCopy( this IHtmlNodeFactory factory, IHtmlComment comment )
    {

      if ( factory == null )
        throw new ArgumentNullException( "factory" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      return factory.CreateComment( comment.Comment );
    }


    /// <summary>
    /// 创建文本节点的副本
    /// </summary>
    /// <param name="factory">用于创建文本节点的构建器</param>
    /// <param name="textNode">需要被创建副本的文本节点</param>
    /// <returns>文本节点的未分配副本</returns>
    public static IFreeTextNode MakeCopy( this IHtmlNodeFactory factory, IHtmlTextNode textNode )
    {
      return factory.CreateTextNode( textNode.HtmlText );
    }



    /// <summary>
    /// 将容器所有内容创建为文档碎片
    /// </summary>
    /// <param name="factory">用于创建节点的创造器</param>
    /// <param name="container">包含内容的容器</param>
    /// <returns>文档碎片</returns>
    public static HtmlFragment MakeFragment( this IHtmlNodeFactory factory, IHtmlContainer container )
    {
      var fragment = new HtmlFragment( factory );

      fragment.AddCopies( container.Nodes() );

      return fragment;
    }




    /*
    /// <summary>
    /// 使用指定文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="text">文本内容</param>
    public static void ReplaceChildsWithText( this IHtmlElement element, string text )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      var factory = element.Document.GetNodeFactory();
      if ( factory == null )
        throw new NotSupportedException();

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( !HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          var fragment = HtmlEncode( text, factory );

          fragment.InsertTo( element, 0 );
        }
        else
        {
          var textNode = factory.CreateTextNode( text );
          textNode.Into( element, 0 );
        }
      }

    }

    /// <summary>
    /// 使用指定的HTML文本替换元素内容（警告，此方法会清除元素所有内容）
    /// </summary>
    /// <param name="element">要替换内容的元素</param>
    /// <param name="html">要替换的HTML代码</param>
    public static void ReplaceChildsWithHtml( this IHtmlElement element, string html )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      var factory = element.Document.GetNodeFactory();
      if ( factory == null )
        throw new NotSupportedException();

      lock ( element.SyncRoot )
      {
        ClearNodes( element );

        if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.OrdinalIgnoreCase ) )
        {
          var textNode = factory.CreateTextNode( html );
          textNode.Into( element, 0 );
        }
        else
        {
          var fragment = factory.ParseFragment( html );
          fragment.InsertTo( element, 0 );
        }
      }
    }
    */





    private static HtmlFragment HtmlEncode( string text, IHtmlNodeFactory factory )
    {

      if ( text == null )
        throw new ArgumentNullException( "text" );

      if ( factory == null )
        throw new ArgumentNullException( "factory" );


      var fragment = new HtmlFragment( factory );
      var encoded = HtmlEncoding.HtmlEncode( text );

      encoded = encoded.Replace( "\r\n", "\n" ).Replace( "\r", "\n" );

      int index = 0, brIndex = 0;
      while ( true )
      {
        brIndex = encoded.IndexOf( '\n', index );

        if ( brIndex == -1 )
        {
          if ( index < encoded.Length )
            fragment.AddNode( factory.CreateTextNode( encoded.Substring( index ) ) );

          break;
        }



        if ( index != brIndex )
          fragment.AddNode( factory.CreateTextNode( encoded.Substring( index, brIndex - index ) ) );
        fragment.AddNode( factory.CreateElement( "br" ) );
        index = brIndex + 1;

      }

      return fragment;
    }


    /*
    /// <summary>
    /// 设置元素的InnerHTML
    /// </summary>
    /// <param name="element">要设置InnerHTML的元素</param>
    /// <param name="html">要设置的HTML文本</param>
    /// <exception cref="System.InvalidOperationException">如果元素不能被安全的修改内容</exception>
    public static void InnerHtml( this IHtmlElement element, string html )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      lock ( element.SyncRoot )
      {
        if ( !IsSafeBindable( element ) )
          throw new InvalidOperationException( "不能对元素设置InnerHTML，因为该元素不能被安全的修改内容。如果确信要改变HTML文档结构，请使用ReplaceChildsWithHtml扩展方法。" );

        ReplaceChildsWithHtml( element, html );
      }
    }


    /// <summary>
    /// 将元素内容替换为指定文本
    /// </summary>
    /// <param name="element">要替换的文本</param>
    /// <param name="text">要设置的文本</param>
    /// <exception cref="System.InvalidOperationException">如果元素不能被安全的修改内容</exception>
    public static void InnerText( this IHtmlElement element, string text )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      lock ( element.SyncRoot )
      {
        if ( !IsSafeBindable( element ) )
          throw new InvalidOperationException( "不能对元素设置InnerText，因为该元素不能被安全的修改内容。如果确信要改变HTML文档结构，请使用ReplaceChildsWithText扩展方法。" );

        ReplaceChildsWithText( element, text );
      }
    }
    */




    /// <summary>
    /// 内部方法，用于确定一个元素的内容是否可以安全数据绑定（不破坏文档结构）。
    /// </summary>
    /// <param name="element">要判断的元素</param>
    /// <returns></returns>
    internal static bool IsSafeBindable( this IHtmlElement element )
    {
      lock ( element.SyncRoot )
      {
        var specification = element.Document.HtmlSpecification;


        if ( element.Nodes().All( n => n is IHtmlTextNode || n is IHtmlComment ) )//只有文本和注释的元素是可以被安全数据绑定的。
          return true;

        if ( !element.Nodes().All( n => n is IHtmlTextNode || n is IHtmlComment || n is IHtmlElement ) )//内容只能由文本、注释和元素三者组成
          return false;


        var childs = element.Elements();


        //如果有任何一个子元素有ID属性，那么它是不安全的
        if ( childs.Any( e => e.Attribute( "id" ).Value() != null ) )
          return false;

        //如果有任何一个子元素是不安全的，那么它是不安全的
        if ( childs.Any( e => e.IsSafeBindable() == false ) )
          return false;

        //如果元素内部只有设置文本样式的子元素，那么它是安全的。
        if ( childs.All( e => specification.IsStylingElement( e ) || specification.IsPhraseElement( e ) ) )
          return true;

        return false;

      }
    }






  }
}
