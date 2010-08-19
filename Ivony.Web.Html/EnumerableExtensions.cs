using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<T> ForAll<T>( this IEnumerable<T> source, Action<T> action )
    {
      foreach ( T item in source )
      {
        action( item );
      }

      return source;
    }

    public static IEnumerable<T> NotNull<T>( this IEnumerable<T> source )
    {
      return source.Where( item => item != null );
    }

    

  }
}
