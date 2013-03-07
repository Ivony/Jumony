using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace Ivony.Data
{

  /// <summary>
  /// 提供分页器和分页数据源的扩展方法
  /// </summary>
  public static class PagingExtension
  {

    /// <summary>
    /// 从序列的指定位置返回指定数量的连续元素
    /// </summary>
    /// <typeparam name="TSource">list 中的元素的类型</typeparam>
    /// <param name="list">要从其返回元素的序列</param>
    /// <param name="start">要返回的元素在序列的开始位置</param>
    /// <param name="count">要返回的元素数量</param>
    /// <returns></returns>
    public static IEnumerable<TSource> Take<TSource>( this IList<TSource> list, int start, int count )
    {
      for ( int index = start; index < Math.Min( start + count, list.Count ); index++ )
      {
        yield return list[index];
      }
    }



    /// <summary>
    /// 从数组中拷贝出一个片段
    /// </summary>
    /// <typeparam name="T">数组元素类型</typeparam>
    /// <param name="source">要从其拷贝的源数组</param>
    /// <param name="start">片段的开始位置</param>
    /// <param name="size">片段的大小</param>
    /// <returns></returns>
    public static T[] CopySegment<T>( this T[] source, int start, int size )
    {
      T[] result = new T[size];
      Array.Copy( source, start, result, 0, size );

      return result;
    }


    /// <summary>
    /// 获取分页器的最大页数
    /// </summary>
    /// <param name="pagingData">数据分页器</param>
    /// <returns></returns>
    public static int MaxPages( this IPagingData pagingData )
    {
      int count = pagingData.Count();
      int pageSize = pagingData.PageSize;

      return count / pageSize + ((count % pageSize) == 0 ? 0 : 1);
    }


    /// <summary>
    /// 从 IEnumerable&lt;T&gt; 创建一个 IPagingDataSource&lt;T&gt;。
    /// </summary>
    /// <typeparam name="T">source 中的元素的类型</typeparam>
    /// <param name="source">数据源</param>
    /// <returns>分页数据源</returns>
    public static IPagingDataSource<T> ToPagingSource<T>( this IEnumerable<T> source )
    {
      if ( source == null )
        throw new ArgumentNullException( "source" );


      var queryable = source as IQueryable<T>;
      if ( queryable != null )
        return ToPagingSource( queryable );


      var pagingSource = source as PagingSourceWrapper<T>;

      if ( pagingSource != null )
        return pagingSource;
      else
        return new PagingSourceWrapper<T>( source );

    }


    /// <summary>
    /// 从 IQueryable&lt;T&gt; 创建一个 IPagingDataSource&lt;T&gt;。
    /// </summary>
    /// <typeparam name="T">source中的元素类型</typeparam>
    /// <param name="source">数据源</param>
    /// <returns>分页数据源</returns>
    public static IPagingDataSource<T> ToPagingSource<T>( this IQueryable<T> source )
    {
      if ( source == null )
        throw new ArgumentNullException( "source" );

      return new QueryPagingSourceWrapper<T>( source );
    }



    /// <summary>
    /// 对分页数据中的每一项进行指定映射操作，得到一个新的分页数据
    /// </summary>
    /// <typeparam name="TSource">源分页数据类型</typeparam>
    /// <typeparam name="TResult">映射后的类型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="selector">映射函数</param>
    /// <returns>执行映射操作后的分页数据</returns>
    public static IPagingData<TResult> Select<TSource, TResult>( this IPagingData<TSource> source, Func<TSource, TResult> selector )
    {
      return new PagingSelector<TSource, TResult>( source, selector );
    }

    /// <summary>
    /// 对分页数据中的每一页进行指定映射操作，得到一个新的分页数据
    /// </summary>
    /// <typeparam name="TSource">源分页数据类型</typeparam>
    /// <typeparam name="TResult">映射后的类型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="pageSelector">映射函数</param>
    /// <returns>执行映射操作后的分页数据</returns>
    public static IPagingData<TResult> PageSelect<TSource, TResult>( this IPagingData<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> pageSelector )
    {
      return new PagingSelector<TSource, TResult>( source, pageSelector );
    }



    /// <summary>
    /// 对分页数据源中的每一项进行指定映射操作，得到一个新的分页数据源
    /// </summary>
    /// <typeparam name="TSource">源分页数据类型</typeparam>
    /// <typeparam name="TResult">映射后的类型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="selector">映射函数</param>
    /// <returns>执行映射操作后的分页数据</returns>
    public static IPagingDataSource<TResult> Select<TSource, TResult>( this IPagingDataSource<TSource> source, Func<TSource, TResult> selector )
    {
      return new PagingSourceSelector<TSource, TResult>( source, selector );
    }

    /// <summary>
    /// 对分页数据源中的每一页进行指定映射操作，得到一个新的分页数据源
    /// </summary>
    /// <typeparam name="TSource">源分页数据类型</typeparam>
    /// <typeparam name="TResult">映射后的类型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="pageSelector">映射函数</param>
    /// <returns>执行映射操作后的分页数据</returns>
    public static IPagingDataSource<TResult> PageSelect<TSource, TResult>( this IPagingDataSource<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> pageSelector )
    {
      return new PagingSourceSelector<TSource, TResult>( source, pageSelector );
    }




    private class PagingSelector<TSource, TResult> : IPagingData<TResult>
    {

      private Func<IEnumerable<TSource>, IEnumerable<TResult>> _pageSelector;
      private Func<TSource, TResult> _selector;

      IPagingData<TSource> _dataSource;

      public PagingSelector( IPagingData<TSource> source, Func<TSource, TResult> selector )
      {
        _dataSource = source;
        _selector = selector;
      }

      public PagingSelector( IPagingData<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> pageConverter )
      {
        _dataSource = source;
        _pageSelector = pageConverter;
      }


      #region IPagingData<TResult> 成员

      public IEnumerable<TResult> GetPage( int pageIndex )
      {
        if ( _pageSelector != null )
          return _pageSelector( _dataSource.GetPage( pageIndex ) );
        else if ( _selector != null )
          return _dataSource.GetPage( pageIndex ).Select( _selector );
        else
          throw new InvalidOperationException();
      }

      IEnumerable IPagingData.GetPage( int pageIndex )
      {
        return GetPage( pageIndex );
      }


      public int Count()
      {
        return _dataSource.Count();
      }

      public int PageSize
      {
        get { return _dataSource.PageSize; }
      }

      #endregion
    }

    private class PagingSourceSelector<TSource, TResult> : IPagingDataSource<TResult>
    {

      private Func<IEnumerable<TSource>, IEnumerable<TResult>> _pageSelector;
      private Func<TSource, TResult> _selector;

      IPagingDataSource<TSource> _dataSource;

      public PagingSourceSelector( IPagingDataSource<TSource> source, Func<TSource, TResult> selector )
      {
        if ( source == null )
          throw new ArgumentNullException( "source" );

        if ( selector == null )
          throw new ArgumentNullException( "selector" );


        _dataSource = source;
        _selector = selector;
      }

      public PagingSourceSelector( IPagingDataSource<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> pageSelector )
      {
        if ( source == null )
          throw new ArgumentNullException( "source" );

        if ( pageSelector == null )
          throw new ArgumentNullException( "pageSelector" );


        _dataSource = source;
        _pageSelector = pageSelector;
      }


      #region IPagingDataSource<TResult> 成员

      public IPagingData<TResult> CreatePaging( int pageSize )
      {

        if ( pageSize < 1 )
          throw new ArgumentOutOfRangeException( "pageSize" );

        if ( _pageSelector != null )
          return _dataSource.CreatePaging( pageSize ).PageSelect( _pageSelector );
        else if ( _selector != null )
          return _dataSource.CreatePaging( pageSize ).Select( _selector );
        else
          throw new InvalidOperationException();
      }

      IPagingData IPagingDataSource.CreatePaging( int pageSize )
      {
        if ( pageSize < 1 )
          throw new ArgumentOutOfRangeException( "pageSize" );

        return CreatePaging( pageSize );
      }

      #endregion
    }

  }


  /// <summary>
  /// 分页数据源包裹类型，用于将列表数据包裹为为分页数据源
  /// </summary>
  /// <typeparam name="T">数据项类型</typeparam>
  public class PagingSourceWrapper<T> : EnumerableWrapper<T>, IPagingDataSource<T>
  {
    private IList<T> _listSource;

    private IEnumerable<T> _enumerableSource;


    /// <summary>
    /// 从一个 IList&lt;T&gt; 列表数据源创建分页数据源
    /// </summary>
    /// <param name="source">列表数据源</param>
    public PagingSourceWrapper( IList<T> source )
      : base( source )
    {
      if ( source == null )
        throw new ArgumentNullException( "source" );

      _listSource = source;
    }

    /// <summary>
    /// 从一个 IEnumerable&lt;T&gt; 列表数据源创建分页数据源
    /// </summary>
    /// <param name="source">列表数据源</param>
    public PagingSourceWrapper( IEnumerable<T> source )
      : base( source )
    {
      if ( source == null )
        throw new ArgumentNullException( "source" );

      _enumerableSource = source;
      _listSource = source as IList<T>;
    }

    #region IPagingDataSource<T> 成员
    
    /// <summary>
    /// 创建分页数据
    /// </summary>
    /// <param name="pageSize">分页大小</param>
    /// <returns>分好页的数据</returns>
    public IPagingData<T> CreatePaging( int pageSize )
    {
      if ( pageSize < 1 )
        throw new ArgumentOutOfRangeException( "pageSize" );

      if ( _listSource != null )
        return new ListPaging<T>( _listSource, pageSize );
      else
        return new EnumerablePaging<T>( _enumerableSource, pageSize );
    }

    IPagingData IPagingDataSource.CreatePaging( int pageSize )
    {
      if ( pageSize < 1 )
        throw new ArgumentOutOfRangeException( "pageSize" );

      return CreatePaging( pageSize );
    }



    #endregion


    private class ListPaging<Q> : IPagingData<Q>
    {

      private IList<Q> _dataSource;
      private int _pageSize;


      public ListPaging( IList<Q> source, int pageSize )
      {
        if ( source == null )
          throw new ArgumentNullException( "source" );

        _dataSource = source;
        _pageSize = pageSize;
      }

      public IEnumerable<Q> GetPage( int pageIndex )
      {
        return _dataSource.Take( (pageIndex - 1) * PageSize, PageSize );
      }

      IEnumerable IPagingData.GetPage( int pageIndex )
      {
        return GetPage( pageIndex );
      }


      public int Count()
      {
        return _dataSource.Count;
      }

      public int PageSize
      {
        get { return _pageSize; }
      }
    }


    private class EnumerablePaging<Q> : IPagingData<Q>
    {

      private IEnumerable<Q> _dataSource;
      private int _pageSize;


      public EnumerablePaging( IEnumerable<Q> source, int pageSize )
      {
        if ( source == null )
          throw new ArgumentNullException( "source" );

        _dataSource = source;
        _pageSize = pageSize;
      }

      public IEnumerable<Q> GetPage( int pageIndex )
      {
        if ( pageIndex < 1 )
          throw new ArgumentOutOfRangeException( "pageIndex" );

        return _dataSource.Skip( (pageIndex - 1) * PageSize ).Take( PageSize );
      }

      IEnumerable IPagingData.GetPage( int pageIndex )
      {
        return GetPage( pageIndex );
      }


      public int Count()
      {
        return _dataSource.Count();
      }

      public int PageSize
      {
        get { return _pageSize; }
      }
    }


  }



  /// <summary>
  /// 分页数据源包裹类型，用于将 IQueryable&lt;T&gt; 数据源包裹为为分页数据源
  /// </summary>
  /// <typeparam name="T">数据项类型</typeparam>
  public class QueryPagingSourceWrapper<T> : IPagingDataSource<T>
  {

    private IQueryable<T> _queryable;

    /// <summary>
    /// 从一个 IQueryable&lt;T&gt; 数据源创建分页数据源
    /// </summary>
    /// <param name="source">数据源</param>
    public QueryPagingSourceWrapper( IQueryable<T> source )
    {
      _queryable = source;
    }


    /// <summary>
    /// 创建分页数据
    /// </summary>
    /// <param name="pageSize">分页大小</param>
    /// <returns>分好页的数据</returns>
    public virtual IPagingData<T> CreatePaging( int pageSize )
    {
      if ( pageSize < 1 )
        throw new ArgumentOutOfRangeException( "pageSize" );

      return new QueryPaging<T>( _queryable, pageSize );
    }

    IPagingData IPagingDataSource.CreatePaging( int pageSize )
    {
      return CreatePaging( pageSize );
    }


    private class QueryPaging<Q> : IPagingData<Q>
    {
      private IQueryable<Q> _queryable;
      private int _pageSize;

      public QueryPaging( IQueryable<Q> queryable, int pageSize )
      {
        _queryable = queryable;
        _pageSize = pageSize;
      }

      public IEnumerable<Q> GetPage( int pageIndex )
      {
        if ( pageIndex < 1 )
          throw new ArgumentOutOfRangeException( "pageIndex" );

        return _queryable.Skip( (pageIndex - 1) * PageSize ).Take( PageSize );
      }

      IEnumerable IPagingData.GetPage( int pageIndex )
      {
        return GetPage( pageIndex );
      }


      public int Count()
      {
        return _queryable.Count();
      }

      public int PageSize
      {
        get { return _pageSize; }
      }
    }

  }



  /// <summary>
  /// 为包装 IEnumerable&lt;T&gt; 提供辅助基类
  /// </summary>
  /// <typeparam name="T">序列的元素类型</typeparam>
  public abstract class EnumerableWrapper<T> : IEnumerable<T>
  {

    private IEnumerable<T> _enumerable;


    /// <summary>
    /// 创建 IEnumerable&lt;T&gt; 的包装
    /// </summary>
    /// <param name="enumerable"></param>
    protected EnumerableWrapper( IEnumerable<T> enumerable )
    {
      _enumerable = enumerable;
    }


    #region IEnumerable<T> 成员

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return _enumerable.GetEnumerator();
    }

    #endregion

    #region IEnumerable 成员

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _enumerable.GetEnumerator();
    }

    #endregion
  }


}
