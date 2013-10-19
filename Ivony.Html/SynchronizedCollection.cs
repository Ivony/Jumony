#pragma warning disable 1591 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{


  /// <summary>
  /// 实现一个线程安全的容器
  /// </summary>
  /// <typeparam name="T">容器项类型</typeparam>
  public class SynchronizedCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
  {
    private List<T> items;
    private object sync;


    /// <summary>
    /// 容器所拥有项数量
    /// </summary>
    public int Count
    {
      get
      {
        lock ( this.sync )
        {
          return items.Count;
        }
      }
    }


    /// <summary>
    /// 内部的容器
    /// </summary>
    protected List<T> Items
    {
      get
      {
        return this.items;
      }
    }


    /// <summary>
    /// 用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get
      {
        return this.sync;
      }
    }


    /// <summary>
    /// 获取指定索引处的元素
    /// </summary>
    /// <param name="index">元素索引</param>
    /// <returns>指定索引位置的元素</returns>
    public T this[int index]
    {
      get
      {
        T result;
        lock ( this.sync )
        {
          result = this.items[index];
        }
        return result;
      }
      set
      {
        lock ( this.sync )
        {
          if ( index < 0 || index >= this.items.Count )
            throw new ArgumentOutOfRangeException( "index" );


          this.SetItem( index, value );
        }
      }
    }


    #region ICollection 显示接口实现
    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return false;
      }
    }


    bool ICollection.IsSynchronized
    {
      get
      {
        return true;
      }
    }


    object ICollection.SyncRoot
    {
      get
      {
        return this.sync;
      }
    }
    #endregion


    #region IList 显示接口实现

    int IList.Add( object value )
    {
      SynchronizedCollection<T>.VerifyValueType( value );
      int result;
      lock ( this.sync )
      {
        this.Add( (T) value );
        result = this.Count - 1;
      }
      return result;
    }

    bool IList.Contains( object value )
    {
      SynchronizedCollection<T>.VerifyValueType( value );
      return this.Contains( (T) value );
    }

    int IList.IndexOf( object value )
    {
      SynchronizedCollection<T>.VerifyValueType( value );
      return this.IndexOf( (T) value );
    }

    void IList.Insert( int index, object value )
    {
      SynchronizedCollection<T>.VerifyValueType( value );
      this.Insert( index, (T) value );
    }

    void IList.Remove( object value )
    {
      SynchronizedCollection<T>.VerifyValueType( value );
      this.Remove( (T) value );
    }

    object IList.this[int index]
    {
      get
      {
        return this[index];
      }
      set
      {
        SynchronizedCollection<T>.VerifyValueType( value );
        this[index] = (T) value;
      }
    }


    bool IList.IsReadOnly
    {
      get
      {
        return false;
      }
    }


    bool IList.IsFixedSize
    {
      get
      {
        return false;
      }
    }
    #endregion



    public SynchronizedCollection() : this( new object() ) { }

    public SynchronizedCollection( object syncRoot ) : this( syncRoot, Enumerable.Empty<T>() ) { }

    public SynchronizedCollection( object syncRoot, IEnumerable<T> list )
    {
      if ( syncRoot == null )
        throw new ArgumentNullException( "syncRoot" );

      if ( list == null )
        throw new ArgumentNullException( "list" );


      this.items = new List<T>( list );
      this.sync = syncRoot;
    }


    public SynchronizedCollection( object syncRoot, params T[] list ) : this( syncRoot, list.AsEnumerable() ) { }


    public void Add( T item )
    {
      lock ( this.sync )
      {
        int count = this.items.Count;
        this.InsertItem( count, item );
      }
    }

    public void Clear()
    {
      lock ( this.sync )
      {
        this.ClearItems();
      }
    }

    public void CopyTo( T[] array, int index )
    {
      lock ( this.sync )
      {
        this.items.CopyTo( array, index );
      }
    }

    public bool Contains( T item )
    {
      bool result;
      lock ( this.sync )
      {
        result = this.items.Contains( item );
      }
      return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.items.GetEnumerator();
    }

    public int IndexOf( T item )
    {
      int result;
      lock ( this.sync )
      {
        result = this.InternalIndexOf( item );
      }
      return result;
    }


    public void Insert( int index, T item )
    {
      lock ( this.sync )
      {
        if ( index < 0 || index > this.items.Count )
          throw new ArgumentOutOfRangeException( "index" );


        this.InsertItem( index, item );
      }
    }


    private int InternalIndexOf( T item )
    {
      int count = this.items.Count;
      for ( int i = 0; i < count; i++ )
      {
        if ( object.Equals( this.items[i], item ) )
        {
          return i;
        }
      }
      return -1;
    }


    public bool Remove( T item )
    {
      bool result;
      lock ( this.sync )
      {
        int num = this.InternalIndexOf( item );
        if ( num < 0 )
        {
          result = false;
        }
        else
        {
          this.RemoveItem( num );
          result = true;
        }
      }
      return result;
    }


    public void RemoveAt( int index )
    {
      lock ( this.sync )
      {
        if ( index < 0 || index >= this.items.Count )
          throw new ArgumentOutOfRangeException( "index" );

        this.RemoveItem( index );
      }
    }


    protected virtual void ClearItems()
    {
      this.items.Clear();

      OnChanged();
    }

    protected virtual void InsertItem( int index, T item )
    {
      this.items.Insert( index, item );

      OnChanged();
    }

    protected virtual void RemoveItem( int index )
    {
      this.items.RemoveAt( index );

      OnChanged();
    }

    protected virtual void SetItem( int index, T item )
    {
      this.items[index] = item;

      OnChanged();
    }

    protected virtual void OnChanged()
    {

    }


    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) this.items).GetEnumerator();
    }

    void ICollection.CopyTo( Array array, int index )
    {
      lock ( this.sync )
      {
        ((ICollection) this.items).CopyTo( array, index );
      }
    }


    private static void VerifyValueType( object value )
    {
      if ( value == null )
      {
        if ( typeof( T ).IsValueType )
        {
          throw new ArgumentException( "元素类型与集合类型不匹配" );
        }
      }
      else
      {
        if ( !(value is T) )
        {
          throw new ArgumentException( "元素类型与集合类型不匹配" );
        }
      }
    }
  }
}
