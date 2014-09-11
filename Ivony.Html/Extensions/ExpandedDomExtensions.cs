using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.ExpandedAPI
{

  /// <summary>
  /// 扩展的 DOM API
  /// </summary>
  public static class ExpandedDomExtensions
  {


    /// <summary>
    /// 在自己后面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其后添加碎片的节点</param>
    /// <param name="fragment">要添加 HTML 碎片</param>
    /// <returns>添加后的节点集</returns>
    public static IEnumerable<IHtmlNode> AddFragmentAfterSelf( this IHtmlNode node, IHtmlFragment fragment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddFragment( node.NodesIndexOfSelf() + 1, fragment );
      }
    }




    /// <summary>
    /// 在自己前面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其前面添加碎片的节点</param>
    /// <param name="fragment">要添加 HTML 碎片</param>
    /// <returns>添加后的节点集</returns>
    public static IEnumerable<IHtmlNode> AddFragmentBeforeSelf( this IHtmlNode node, IHtmlFragment fragment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( fragment == null )
        throw new ArgumentNullException( "fragment" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddFragment( node.NodesIndexOfSelf(), fragment );
      }
    }

    /// <summary>
    /// 在自己后面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其后添加碎片的节点</param>
    /// <param name="html">要分析成碎片的 HTML 文本</param>
    /// <returns>添加后的节点集</returns>
    public static IEnumerable<IHtmlNode> AddFragmentAfterSelf( this IHtmlNode node, string html )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( html == null )
        throw new ArgumentNullException( "html" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddFragment( node.NodesIndexOfSelf() + 1, html );
      }
    }



    /// <summary>
    /// 在自己前面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其前面添加碎片的节点</param>
    /// <param name="html">要分析成碎片的 HTML 文本</param>
    /// <returns>添加后的节点集</returns>
    public static IEnumerable<IHtmlNode> AddFragmentBeforeSelf( this IHtmlNode node, string html )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( html == null )
        throw new ArgumentNullException( "html" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddFragment( node.NodesIndexOfSelf(), html );
      }
    }





    /// <summary>
    /// 在自己后面添加一个注释
    /// </summary>
    /// <param name="node">要在其后添加注释的节点</param>
    /// <param name="comment">要添加 HTML 注释内容</param>
    /// <returns>添加的注释节点</returns>
    public static IHtmlComment AddCommentAfterSelf( this IHtmlNode node, string comment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddComment( node.NodesIndexOfSelf() + 1, comment );
      }
    }


    /// <summary>
    /// 在自己前面添加一个注释
    /// </summary>
    /// <param name="node">要在其前面添加注释的节点</param>
    /// <param name="comment">要添加 HTML 注释内容</param>
    /// <returns>添加的注释节点</returns>
    public static IHtmlComment AddCommentBeforeSelf( this IHtmlNode node, string comment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddComment( node.NodesIndexOfSelf(), comment );
      }
    }


    /// <summary>
    /// 在自己后面添加一个文本节点
    /// </summary>
    /// <param name="node">要在其后添加文本的节点</param>
    /// <param name="htmlText">要添加 HTML 文本</param>
    /// <returns>添加的文本节点</returns>
    public static IHtmlTextNode AddTextNodeAfterSelf( this IHtmlNode node, string htmlText )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddTextNode( node.NodesIndexOfSelf() + 1, htmlText );
      }
    }


    /// <summary>
    /// 在自己前面添加一个文本节点
    /// </summary>
    /// <param name="node">要在其前面添加文本的节点</param>
    /// <param name="htmlText">要添加 HTML 文本</param>
    /// <returns>添加的文本节点</returns>
    public static IHtmlTextNode AddTextNodeBeforSelf( this IHtmlNode node, string htmlText )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container )
      {
        return container.AddTextNode( node.NodesIndexOfSelf(), htmlText );
      }
    }





    /// <summary>
    /// 在自己后面添加一个元素
    /// </summary>
    /// <param name="node">要在其后添加元素的节点</param>
    /// <param name="elementName">添加的元素名</param>
    /// <returns>添加的元素</returns>
    public static IHtmlElement AddElementAfterSelf( this IHtmlNode node, string elementName )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container.SyncRoot )
      {
        return container.AddElement( node.NodesIndexOfSelf() + 1, elementName );
      }
    }


    /// <summary>
    /// 在自己前面添加一个元素
    /// </summary>
    /// <param name="node">要在其前面添加元素的节点</param>
    /// <param name="elementName">添加的元素名</param>
    /// <returns>添加的元素</returns>
    public static IHtmlElement AddElementBeforeSelf( this IHtmlNode node, string elementName )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );

      var container = node.Container;


      if ( container == null )
        throw new InvalidOperationException();

      lock ( container )
      {
        return container.AddElement( node.NodesIndexOfSelf(), elementName );
      }
    }



    /// <summary>
    /// 在后面添加节点的副本
    /// </summary>
    /// <param name="node">要在其后面添加副本的节点</param>
    /// <param name="textNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlTextNode AddCopyAfterSelf( this IHtmlNode node, IHtmlTextNode textNode )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf() + 1, textNode );
      }
    }



    /// <summary>
    /// 在前面添加节点的副本
    /// </summary>
    /// <param name="node">要在其前面添加副本的节点</param>
    /// <param name="textNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlTextNode AddCopyBeforeSelf( this IHtmlNode node, IHtmlTextNode textNode )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( textNode == null )
        throw new ArgumentNullException( "textNode" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf(), textNode );
      }
    }



    /// <summary>
    /// 在后面添加节点的副本
    /// </summary>
    /// <param name="node">要在其后面添加副本的节点</param>
    /// <param name="comment">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlComment AddCopyAfterSelf( this IHtmlNode node, IHtmlComment comment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf() + 1, comment );
      }
    }



    /// <summary>
    /// 在前面添加节点的副本
    /// </summary>
    /// <param name="node">要在其前面添加副本的节点</param>
    /// <param name="comment">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlComment AddCopyBeforeSelf( this IHtmlNode node, IHtmlComment comment )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf(), comment );
      }
    }



    /// <summary>
    /// 在后面添加节点的副本
    /// </summary>
    /// <param name="node">要在其后面添加副本的节点</param>
    /// <param name="element">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlElement AddCopyAfterSelf( this IHtmlNode node, IHtmlElement element )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( element == null )
        throw new ArgumentNullException( "element" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf() + 1, element );
      }
    }



    /// <summary>
    /// 在前面添加节点的副本
    /// </summary>
    /// <param name="node">要在其前面添加副本的节点</param>
    /// <param name="element">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlElement AddCopyBeforeSelf( this IHtmlNode node, IHtmlElement element )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( element == null )
        throw new ArgumentNullException( "element" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf(), element );
      }
    }



    /// <summary>
    /// 在后面添加节点的副本
    /// </summary>
    /// <param name="node">要在其后面添加副本的节点</param>
    /// <param name="newNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlNode AddCopyAfterSelf( this IHtmlNode node, IHtmlNode newNode )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( newNode == null )
        throw new ArgumentNullException( "newNode" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf() + 1, newNode );
      }
    }



    /// <summary>
    /// 在前面添加节点的副本
    /// </summary>
    /// <param name="node">要在其前面添加副本的节点</param>
    /// <param name="newNode">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IHtmlNode AddCopyBeforeSelf( this IHtmlNode node, IHtmlNode newNode )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( newNode == null )
        throw new ArgumentNullException( "newNode" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf(), newNode );
      }
    }



    /// <summary>
    /// 在后面添加节点的副本
    /// </summary>
    /// <param name="node">要在其后面添加副本的节点</param>
    /// <param name="newNodes">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IEnumerable<IHtmlNode> AddCopyAfterSelf( this IHtmlNode node, IEnumerable<IHtmlNode> newNodes )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( newNodes == null )
        throw new ArgumentNullException( "newNodes" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf() + 1, newNodes );
      }
    }



    /// <summary>
    /// 在前面添加节点的副本
    /// </summary>
    /// <param name="node">要在其前面添加副本的节点</param>
    /// <param name="newNodes">要创作副本的节点</param>
    /// <returns>添加后的节点</returns>
    public static IEnumerable<IHtmlNode> AddCopyBeforeSelf( this IHtmlNode node, IEnumerable<IHtmlNode> newNodes )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( newNodes == null )
        throw new ArgumentNullException( "newNodes" );


      var container = node.Container;

      lock ( container.SyncRoot )
      {
        return container.AddCopy( node.NodesIndexOfSelf(), newNodes );
      }
    }



    /// <summary>
    /// 将元素重复指定次数
    /// </summary>
    /// <param name="element">要重复的元素</param>
    /// <param name="count">要重复的次数</param>
    /// <returns>所产生的元素序列</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">重复的次数小于 0</exception>
    /// <remarks>
    /// 若提供的 count 等于 0，则会从 DOM中 移除元素，若等于 1 则不做任何事情。
    /// </remarks>
    public static IHtmlElement[] Repeat( this IHtmlElement element, int count )
    {

      if ( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      switch ( count )
      {
        case 0:
          element.Remove();
          return new IHtmlElement[0];
        case 1:
          return new[] { element };

        default:

          IHtmlElement[] result = new IHtmlElement[count];
          result[0] = element;

          var container = element.Container;
          lock ( element.Container )
          {
            var index = element.NodesIndexOfSelf();
            for ( int i = 1; i < count; i++ )
              result[i] = container.AddCopy( index + i, element );
          }

          return result;
      }
    }


  }
}
