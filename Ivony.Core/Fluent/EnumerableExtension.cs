using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

namespace Ivony.Fluent
{
  public static class EnumerableExtension
  {




    private static readonly Random random = new Random( DateTime.Now.Millisecond );

    public static T RandomElement<T>( this IEnumerable<T> source )
    {
      return source.ElementAt( random.Next( source.Count() ) );
    }






    public static IEnumerable<T> ForAll<T>( this IEnumerable<T> source, Action<T> action )
    {
      foreach ( T item in source )
      {
        action( item );
      }

      return source;
    }


    public static IEnumerable<T> ForAll<T>( this IEnumerable<T> source, Action<T, int> action )
    {

      int i = 0;

      foreach ( T item in source )
      {
        action( item, i++ );
      }

      return source;
    }


    public static IEnumerable<T> NotNull<T>( this IEnumerable<T> source )
    {
      return source.Where( item => item != null );
    }


    public static bool OnlyOne<T>( this IEnumerable<T> source )
    {

      var onlyone = false;

      foreach ( var item in source )
      {
        if ( onlyone )
          return false;

        onlyone = true;
      }

      return onlyone;

    }





    /// <summary>
    /// 将集合的每一项按顺序绑定到另一个集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TSource> BindTo<TSource, TTarget>( this IEnumerable<TSource> source, IEnumerable<TTarget> targets, Action<TSource, TTarget> binder )
    {
      return BindTo( source, targets, ( d, t, i ) => binder( d, t ) );
    }


    /// <summary>
    /// 将集合的每一项按顺序绑定到另一个集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TSource> BindTo<TSource, TTarget>( this IEnumerable<TSource> source, IEnumerable<TTarget> targets, Action<TSource, TTarget, int> binder )
    {

      BindCore( source, targets, binder );

      return source;
    }


    /// <summary>
    /// 将集合的每一项按顺序绑定到另一个集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="defaultValue">当源集合元素不够时所采用的默认元素</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TSource> BindTo<TSource, TTarget>( this IEnumerable<TSource> source, IEnumerable<TTarget> targets, TSource defaultValue, Action<TSource, TTarget> binder )
    {
      return BindTo( source, targets, defaultValue, ( d, t, i ) => binder( d, t ) );
    }


    /// <summary>
    /// 将集合的每一项按顺序绑定到另一个集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="defaultValue">当源集合元素不够时所采用的默认元素</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TSource> BindTo<TSource, TTarget>( this IEnumerable<TSource> source, IEnumerable<TTarget> targets, TSource defaultValue, Action<TSource, TTarget, int> binder )
    {
      BindCore( source, targets, defaultValue, binder );

      return source;
    }





    /// <summary>
    /// 将源集合的每一项按顺序绑定到集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TTarget> BindFrom<TSource, TTarget>( this IEnumerable<TTarget> targets, IEnumerable<TSource> source, Action<TSource, TTarget> binder )
    {
      return BindFrom( targets, source, ( s, t, i ) => binder( s, t ) );
    }


    /// <summary>
    /// 将源集合的每一项按顺序绑定到集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TTarget> BindFrom<TSource, TTarget>( this  IEnumerable<TTarget> targets, IEnumerable<TSource> source, Action<TSource, TTarget, int> binder )
    {

      BindCore( source, targets, binder );

      return targets;
    }


    /// <summary>
    /// 将源集合的每一项按顺序绑定到集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="defaultValue">当源集合元素不够时所采用的默认元素</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TTarget> BindFrom<TSource, TTarget>( this IEnumerable<TTarget> targets, IEnumerable<TSource> source, TSource defaultValue, Action<TSource, TTarget> binder )
    {
      return BindFrom( targets, source, defaultValue, ( s, t, i ) => binder( s, t ) );
    }


    /// <summary>
    /// 将源集合的每一项按顺序绑定到集合
    /// </summary>
    /// <typeparam name="TSource">源集合元素类型</typeparam>
    /// <typeparam name="TTarget">目标集合元素类型</typeparam>
    /// <param name="source">源集合</param>
    /// <param name="targets">目标集合</param>
    /// <param name="defaultValue">当源集合元素不够时所采用的默认元素</param>
    /// <param name="binder">绑定方法</param>
    /// <returns>源集合</returns>
    public static IEnumerable<TTarget> BindFrom<TSource, TTarget>( this IEnumerable<TTarget> targets, IEnumerable<TSource> source, TSource defaultValue, Action<TSource, TTarget, int> binder )
    {
      BindCore( source, targets, defaultValue, binder );

      return targets;
    }











    private static void BindCore<TSource, TTarget>( IEnumerable<TSource> source, IEnumerable<TTarget> targets, Action<TSource, TTarget, int> binder )
    {

      using ( var sourceIterator = source.GetEnumerator() )
      {
        using ( var targetIterator = targets.GetEnumerator() )
        {
          int index = 0;

          while ( sourceIterator.MoveNext() && targetIterator.MoveNext() )
            binder( sourceIterator.Current, targetIterator.Current, index++ );
        }
      }
    }


    private static void BindCore<TSource, TTarget>( IEnumerable<TSource> source, IEnumerable<TTarget> targets, TSource defaultValue, Action<TSource, TTarget, int> binder )
    {

      using ( var sourceIterator = source.GetEnumerator() )
      {
        using ( var targetIterator = targets.GetEnumerator() )
        {

          bool sourceEnded = false;

          while ( targetIterator.MoveNext() )
          {

            int index = 0;

            if ( !sourceEnded )
              sourceEnded = !sourceIterator.MoveNext();

            var dataItem = sourceEnded ? defaultValue : sourceIterator.Current;
            var targetItem = targetIterator.Current;

            binder( dataItem, targetItem, index++ );

          }
        }
      }
    }




    /// <summary>
    /// 创建只读枚举封装，避免集合类型强制类型转换后被修改
    /// </summary>
    /// <typeparam name="T">枚举元素类型</typeparam>
    /// <param name="enumerable">要创建只读枚举封装的集合</param>
    /// <returns>只读枚举封装</returns>
    public static IEnumerable<T> AsReadOnly<T>( this IEnumerable<T> enumerable )
    {
      return new ReadOnlyEnumerable<T>( enumerable );
    }


    public static IList<T> AsReadOnly<T>( this IList<T> list )
    {
      return new ReadOnlyCollection<T>( list );
    }


  }
}
