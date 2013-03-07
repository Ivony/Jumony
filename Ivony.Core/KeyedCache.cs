using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ivony
{
  public class KeyedCache<TKey, TValue>
  {

    private Dictionary<TKey,TValue> _cache;
    private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    /// <summary>
    /// 创建 KeyedCache 实例
    /// </summary>
    public KeyedCache()
    {
      _cache = new Dictionary<TKey, TValue>();
    }


    /// <summary>
    /// 创建 KeyedCache 实例
    /// </summary>
    /// <param name="comparer">比较键时要使用的 System.Collections.Generic.IEqualityComparer&gt;T&lt; 实现</param>
    public KeyedCache( IEqualityComparer<TKey> comparer )
    {
      _cache = new Dictionary<TKey, TValue>( comparer );
    }

    /// <summary>
    /// 从缓存中获取或创建新项
    /// </summary>
    /// <param name="key">缓存键</param>
    /// <param name="creator">用于创建新项的创建器</param>
    /// <returns>从缓存中获取或者创建的项</returns>
    public TValue FetchOrCreateItem( TKey key, Func<TValue> creator )
    {
      _lock.EnterReadLock();
      TValue result;
      try
      {
        TValue value;
        if ( this._cache.TryGetValue( key, out value ) )
        {
          result = value;
          return result;
        }
      }
      finally
      {
        this._lock.ExitReadLock();
      }


      {
        TValue value = creator();
        
        this._lock.EnterWriteLock();
        try
        {
          TValue _value;
          if ( this._cache.TryGetValue( key, out _value ) )
            return _value;

          else
            return _cache[key] = value;
        }
        finally
        {
          _lock.ExitWriteLock();
        }

      }
    }
  }
}
