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


    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }
  }

}
