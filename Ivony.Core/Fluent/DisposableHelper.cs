using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  public static class DisposableHelper
  {
    public static IDisposable AsDisposable<T>( this T obj, Action<T> disposeMethod )
    {
      if ( obj == null )
        return null;

      return new DisposableWrapper<T>( obj, disposeMethod );
    }

    public class DisposableWrapper<T> : IDisposable
    {
      private T _obj;
      private Action<T> _disposableMethod;

      public DisposableWrapper( T obj, Action<T> disposeMethod )
      {
        if ( disposeMethod == null )
          throw new ArgumentNullException( "disposeMethod" );

        _disposableMethod = disposeMethod;
        _obj = obj;
      }

      public void Dispose()
      {
        _disposableMethod( _obj );
      }


      public static implicit operator T( DisposableWrapper<T> wrapper )
      {
        return wrapper._obj;
      }
    }

  }
}
