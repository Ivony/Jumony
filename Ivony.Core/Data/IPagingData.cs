using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Data
{

  /// <summary>
  /// 定义一个数据分页器
  /// </summary>
  public interface IPagingData
  {
    /// <summary>
    /// 获取分页后的数据
    /// </summary>
    /// <param name="pageIndex">页序号</param>
    /// <returns></returns>
    IEnumerable GetPage( int pageIndex );

    /// <summary>
    /// 获取数据总数
    /// </summary>
    /// <returns></returns>
    int Count();

    /// <summary>
    /// 页大小
    /// </summary>
    int PageSize
    {
      get;
    }
  }

  /// <summary>
  /// 定义一个数据分页器
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IPagingData<T> : IPagingData
  {
    /// <summary>
    /// 获取分页后的数据
    /// </summary>
    /// <param name="pageIndex">页序号</param>
    /// <returns></returns>
    new IEnumerable<T> GetPage( int pageIndex );
  }


  /// <summary>
  /// 可分页数据源
  /// </summary>
  public interface IPagingDataSource
  {
    /// <summary>
    /// 创建数据分页器
    /// </summary>
    /// <param name="pageSize">页大小</param>
    /// <returns>数据分页器</returns>
    IPagingData CreatePaging( int pageSize );
  }

  /// <summary>
  /// 可分页数据源
  /// </summary>
  /// <typeparam name="T">数据元素类型</typeparam>
  public interface IPagingDataSource<T> : IPagingDataSource
  {
    /// <summary>
    /// 创建分页器
    /// </summary>
    /// <param name="pageSize">分页大小</param>
    /// <returns>数据分页器</returns>
    new IPagingData<T> CreatePaging( int pageSize );
  }
}
