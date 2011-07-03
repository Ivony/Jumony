using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Templates
{
  public static class TemplateExtensions
  {

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
    public static IEnumerable<IHtmlElement> Repeat( this IHtmlElement element, int count )
    {

      if ( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      switch ( count )
      {
        case 0:
          element.Remove();
          return Enumerable.Empty<IHtmlElement>();
        case 1:
          return new[] { element };

        default:

          List<IHtmlElement> result = new List<IHtmlElement>() { element };

          var container = element.Container;
          lock ( element.Container )
          {
            var index = element.NodesIndexOfSelf();
            for ( int i = 1; i < count; i++ )
              result.Add( container.AddCopy( index + i, element ) );
          }

          return result;

      }
    }

  }
}
