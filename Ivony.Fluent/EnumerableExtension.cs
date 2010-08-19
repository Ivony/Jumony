using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  public static class EnumerableExtension
  {

    public static void ForEach<T>( this IEnumerable<T> source, Action<T> action )
    {
      foreach ( var item in source )
        action( item );
    }
  }
}
