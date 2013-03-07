using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 提供节点定位、相对位置查询的扩展方法
  /// </summary>
  public static class LocationExtensions
  {



    /// <summary>
    /// 判断指定节点是否为指定容器的祖先。
    /// </summary>
    /// <param name="container">要判断的容器</param>
    /// <param name="node">要判断的节点</param>
    /// <returns>若节点位于容器的子代，则返回 true ，否则返回 false 。</returns>
    /// <remarks>注意此操作和IsDescendantOf并非互逆，当两个节点没有任何关系或者为同一节点时，两者皆返回false，此方法其实只是IsDescendantOf的参数颠倒版。</remarks>
    public static bool IsAncestorOf( this IHtmlContainer container, IHtmlNode node )
    {
      return IsDescendantOf( node, container );
    }


    /// <summary>
    /// 判断指定节点是否为指定容器的子代。
    /// </summary>
    /// <param name="node">要判断的节点</param>
    /// <param name="container">要判断的容器</param>
    /// <returns>若节点位于容器的子代，则返回 true ，否则返回 false 。</returns>
    public static bool IsDescendantOf( this IHtmlNode node, IHtmlContainer container )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      var collection = container as IHtmlCollection;

      if ( collection != null )
        return IsDescendantOf( node, collection );


      while ( true )
      {
        if ( node.Container == null )
          return false;

        if ( object.Equals( node.Container, container ) )
          return true;

        node = node.Container as IHtmlNode;

        if ( node == null )
          return false;
      }
    }


    /// <summary>
    /// 判断指定节点是否为指定容器的子代。
    /// </summary>
    /// <param name="node">要判断的节点</param>
    /// <param name="collection">要判断的容器</param>
    /// <returns>若节点位于容器的子代，则返回 true ，否则返回 false 。</returns>
    /// <remarks>
    /// 出于性能考虑， IsDescendantOf( this node, container ) 方法检查节点的所有父级是否包含指定的容器，但对于IHtmlCollection来说，即使节点是其子代，其也不会在其父级中出现。
    /// 所以这是针对 IHtmlCollection 的一个特定实现，而 IsDescendantOf( this IHtmlNode, IHtmlContainer ) 方法发现第二个参数是IHtmlCollection时，也会自动调用此重载
    /// </remarks>
    internal static bool IsDescendantOf( this IHtmlNode node, IHtmlCollection collection )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( collection == null )
        throw new ArgumentNullException( "collection" );

      if ( collection.Nodes().Contains( node ) )
        return true;

      if ( collection.Nodes().OfType<IHtmlContainer>().Any( child => node.IsDescendantOf( child ) ) )
        return true;

      return false;
    }



    /// <summary>
    /// 获取元素相对于文档根的路径表达
    /// </summary>
    /// <param name="element">要获取路径的元素</param>
    /// <returns>路径表达式</returns>
    public static string PathOf( this IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      element.EnsureAllocated();

      return PathOf( element, element.Document );

    }

    /// <summary>
    /// 获取元素相对于指定元素的路径表达
    /// </summary>
    /// <param name="element">要获取路径的文档</param>
    /// <param name="ancestor">计算路径的起始对象</param>
    /// <returns>路径表达式</returns>
    public static string PathOf( this IHtmlElement element, IHtmlContainer ancestor )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( ancestor == null )
        throw new ArgumentNullException( "ancestor" );


      element.EnsureAllocated();

      StringBuilder builder = new StringBuilder();

      while ( true )
      {
        builder.Insert( 0, "/" + element.Name );


        var container = element.Container;

        if ( container.Equals( ancestor ) )
          return builder.ToString();


        element = container as IHtmlElement;

        if ( element == null )
          return null;
      }
    }





    private class NodeLocationComparerImplements : IComparer<IHtmlNode>
    {
      public int Compare( IHtmlNode x, IHtmlNode y )
      {
        if ( x == null )
          throw new ArgumentNullException( "x" );

        if ( y == null )
          throw new ArgumentNullException( "y" );

        if ( !object.Equals( x.Document, y.Document ) )
          throw new InvalidOperationException();

        x.EnsureAllocated();
        y.EnsureAllocated();

        if ( object.Equals( x, y ) )
          return 0;

        if ( object.Equals( x.Container, y.Container ) )
          return x.NodesIndexOfSelf() - y.NodesIndexOfSelf();


        var ancetors1 = x.Ancestors().Reverse().ToArray();
        var ancetors2 = y.Ancestors().Reverse().ToArray();

        int i = 0;
        while ( true )
        {

          if ( i > ancetors1.Length )
            return -1;

          if ( i > ancetors2.Length )
            return 1;

          if ( !object.Equals( ancetors1[i], ancetors2[i] ) )
            break;
        }

        return ancetors1[i].NodesIndexOfSelf() - ancetors2[i].NodesIndexOfSelf();
      }
    }



    private static NodeLocationComparerImplements locationComparer = new NodeLocationComparerImplements();

    /// <summary>
    /// 获取一个节点位置比较器，可以比较同一文档上节点在文档上出现的位置。
    /// </summary>
    public static IComparer<IHtmlNode> NodeLocationComparer
    {
      get { return locationComparer; }
    }
  }
}
