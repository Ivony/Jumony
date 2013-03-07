using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  /// <summary>
  /// 为自动销毁对象逻辑提供流畅的写法的扩展方法
  /// </summary>
  public static class DisposableHelper
  {

    /// <summary>
    /// 对没有实现 IDisposable 的对象返回实现了 IDisposable 的包装。
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="obj">要包装的对象</param>
    /// <param name="disposeMethod">Dispose方法</param>
    /// <returns>包装对象</returns>
    public static DisposableWrapper<T> AsDisposable<T>( this T obj, Action<T> disposeMethod )
    {
      if ( obj == null )
        return null;

      return new DisposableWrapper<T>( obj, disposeMethod );
    }

    /// <summary>
    /// 包装没有实现 IDisposable 的对象，使其支持 using
    /// </summary>
    /// <typeparam name="T">包装的对象类型</typeparam>
    public sealed class DisposableWrapper<T> : IDisposable
    {
      private T _obj;
      private Action<T> _disposableMethod;

      internal DisposableWrapper( T obj, Action<T> disposeMethod )
      {
        if ( disposeMethod == null )
          throw new ArgumentNullException( "disposeMethod" );

        _disposableMethod = disposeMethod;
        _obj = obj;
      }

      
      void IDisposable.Dispose()
      {
        _disposableMethod( _obj );
      }


      /// <summary>
      /// 重载隐式类型转换运算符，使得包装对象可以被隐式类型转换为被包装对象
      /// </summary>
      /// <param name="wrapper">包装对象</param>
      /// <returns>被包装的对象</returns>
      public static implicit operator T( DisposableWrapper<T> wrapper )
      {
        return wrapper._obj;
      }
    }

  }
}
