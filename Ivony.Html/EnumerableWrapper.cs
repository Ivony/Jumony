using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Html
{
  /// <summary>
  /// 提供一个 IEnumerable 对象的包装
  /// </summary>
  public abstract class EnumerableWrapper : IEnumerable
  {

    /// <summary>
    /// 获取被包装的可枚举序列
    /// </summary>
    /// <returns>可枚举序列</returns>
    protected abstract IEnumerable GetEnumerable();


    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }
  }


  /// <summary>
  /// 提供一个 IEnumerable&lt;T&gt; 对象的包装
  /// </summary>
  /// <typeparam name="T">元素类型</typeparam>
  public abstract class EnumerableWrapper<T> : IEnumerable<T>
  {

    /// <summary>
    /// 获取被包装的可枚举序列
    /// </summary>
    /// <returns>可枚举序列</returns>
    protected abstract IEnumerable<T> GetEnumerable();


    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

    /// <summary>
    /// 获取枚举器
    /// </summary>
    /// <returns>枚举器</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }
  }

}
