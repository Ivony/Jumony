using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.ExpandedAPI;


namespace Ivony.Html.Templates
{
  public static class TemplateExtensions
  {


#if DEBUG
    /// <summary>
    /// 以选择器选择的结果当作模版项，调整模版项重复数量
    /// </summary>
    /// <param name="container">应被视为模版根的容器</param>
    /// <param name="selector">CSS选择器，用于选择模版项</param>
    /// <param name="count">模版项的重复数量</param>
    /// <returns>调整数量后的模版项集合</returns>
    public static IEnumerable<IHtmlElement> Repeat( this IHtmlContainer container, string selector, int count )
    {
      var items = container.Find( selector ).ToArray();

      {
        IHtmlElement lastItem = null;

        foreach ( var i in items )
        {
          if ( lastItem != null && i.IsDescendantOf( lastItem ) )
            throw new InvalidOperationException( "模版项不能相互包含，请检查所提供的选择器" );

          lastItem = i;
        }
      }

      lock ( container.SyncRoot )
      {

        var firstItem = items.First();
        var firstContainer = container.Elements().First( e => e.IsAncestorOf( firstItem ) || e.Equals( firstItem ) );

        var lastItem = items.Last();
        var lastContainer = container.Elements().First( e => e.IsAncestorOf( lastItem ) || e.Equals( lastItem ) );

        var firstContainerIndex = firstContainer.NodesIndexOfSelf();
        var lastContainerIndex = lastContainer.NodesIndexOfSelf();


        while ( true )
        {

          if ( items.Length == count )
            return items;

          if ( items.Length > count )
            break;


          var range = container.Nodes().SkipWhile( n => !n.Equals( firstContainer.NextNode() ) ).TakeWhile( n => !n.Equals( lastContainer.NextNode() ) ).ToArray();

          lastContainer.AddCopyAfterSelf( range );

          items = container.Find( selector ).ToArray();

        }


        lastItem = items.Last();
        lastContainer = container.Elements().First( e => e.IsAncestorOf( lastItem ) || e.Equals( lastItem ) );



        var availableItems = items.Take( count ).ToArray();

        if ( items.Length > count )
        {
          var lastAvailableItem = items[count - 1];//因为数组下标从0开始，故此处不必+1。
          var lastAvailableContainer = container.Elements().First( e => e.IsAncestorOf( lastAvailableItem ) || e.Equals( lastAvailableItem ) );


          container.Nodes()
            .SkipWhile( n => !n.Equals( lastAvailableContainer.NextNode() ) )
            .TakeWhile( n => !n.Equals( lastContainer.NextNode() ) )
            .Remove();

        }

        return availableItems;

      }

    }
#endif




    private static bool ContainsAnyOf( this IHtmlContainer container, IEnumerable<IHtmlNode> nodes )
    {
      return nodes.Any( n => n.IsDescendantOf( container ) || n.Equals( container ) );
    }

  }
}
