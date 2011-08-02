using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public static class ExternDomExtensions
  {




    /// <summary>
    /// 在自己后面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其后添加碎片的节点</param>
    /// <param name="elementName">要添加 HTML 碎片</param>
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
    /// <param name="elementName">要添加 HTML 碎片</param>
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
    /// <param name="elementName">要分析成碎片的 HTML 文本</param>
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
        return container.AddFragment( node.NodesIndexOfSelf(), html );
      }
    }



    /// <summary>
    /// 在自己前面添加一个文档碎片
    /// </summary>
    /// <param name="node">要在其前面添加碎片的节点</param>
    /// <param name="elementName">要分析成碎片的 HTML 文本</param>
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
    /// <param name="elementName">要添加 HTML 文本</param>
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
    /// 在自己后面添加一个注释
    /// </summary>
    /// <param name="node">要在其前面添加注释的节点</param>
    /// <param name="elementName">要添加 HTML 文本</param>
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
    /// <param name="elementName">要添加 HTML 文本</param>
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
    /// <param name="elementName">要添加 HTML 文本</param>
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
    /// <param name="container">要添加副本的容器</param>
    /// <param name="comment">要创作副本的节点</param>
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
    /// <param name="container">要添加副本的容器</param>
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
    /// <param name="container">要添加副本的容器</param>
    /// <param name="textNode">要创作副本的节点</param>
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
    /// <param name="container">要添加副本的容器</param>
    /// <param name="textNode">要创作副本的节点</param>
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



  }
}
