using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{

  internal class SuperLinkedList<T> : ICollection<T>
  {
    private LinkedList<T> _list = new LinkedList<T>();

    private Dictionary<T, LinkedListNode<T>> _index = new Dictionary<T, LinkedListNode<T>>();



    private object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }


    public void Add( T item )
    {
      lock ( SyncRoot )
      {
        _cache = null;

        _index.Add( item, _list.AddLast( item ) );
      }
    }

    public void AddBefore( T item, T addedItem )
    {
      if ( item == null )
        throw new ArgumentNullException( "item" );

      if ( addedItem == null )
        throw new ArgumentNullException( "addedItem" );

      lock ( SyncRoot )
      {
        _cache = null;

        LinkedListNode<T> node;
        if ( !_index.TryGetValue( item, out node ) )
          throw new InvalidOperationException();

        _index.Add( addedItem, _list.AddBefore( node, addedItem ) );
      }
    }


    public void AddAfter( T item, T addedItem )
    {

      if ( item == null )
        throw new ArgumentNullException( "item" );

      if ( addedItem == null )
        throw new ArgumentNullException( "addedItem" );

      lock ( SyncRoot )
      {
        _cache = null;

        LinkedListNode<T> node;
        if ( !_index.TryGetValue( item, out node ) )
          throw new InvalidOperationException();

        var _node = _list.AddAfter( node, addedItem );
        _index.Add( addedItem, _node );
      }
    }


    public T LastItem { get { return _list.Last.Value; } }

    public T FirstItem { get { return _list.First.Value; } }



    public void Clear()
    {
      lock ( SyncRoot )
      {
        _index.Clear();
        _list.Clear();
      }
    }

    public bool Contains( T item )
    {
      lock ( SyncRoot )
      {
        return _index.ContainsKey( item );
      }
    }

    public void CopyTo( T[] array, int arrayIndex )
    {
      _list.CopyTo( array, arrayIndex );
    }

    public int Count
    {
      get { return _index.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove( T item )
    {
      lock ( SyncRoot )
      {

        _cache = null;

        LinkedListNode<T> node;
        if ( !_index.TryGetValue( item, out node ) )
          return false;

        _index.Remove( item );
        _list.Remove( node );
        return true;
      }
    }


    private T[] _cache;

    public IEnumerator<T> GetEnumerator()
    {

      lock ( SyncRoot )
      {

        if ( _cache != null )
          return _cache.CastTo<IEnumerable<T>>().GetEnumerator();

        return (_cache = _list.ToArray()).CastTo<IEnumerable<T>>().GetEnumerator();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
