using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 定义一个数据分页器
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IPagingData<T>
  {
    /// <summary>
    /// 获取分页后的数据
    /// </summary>
    /// <param name="pageIndex">页序号</param>
    /// <returns></returns>
    IEnumerable<T> GetPage( int pageIndex );

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
  /// 可分页数据源
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IPagingDataSource<T>
  {
    /// <summary>
    /// 创建分页器
    /// </summary>
    /// <param name="pageSize">分页大小</param>
    /// <returns></returns>
    IPagingData<T> CreatePaging( int pageSize );
  }
}
