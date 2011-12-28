using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Indexing
{



  public interface IElementIndexer
  {
    void IndexElement( IHtmlElement element );
  }


  /// <summary>
  /// 文档的 ID 索引
  /// </summary>
  public class ElementIdentityIndex : IDictionary<string, IHtmlElement>
  {
    internal ElementIdentityIndex( IHtmlDocument document )
    {

    }


    private Dictionary<string, IHtmlElement> data = new Dictionary<string, IHtmlElement>();



    /// <summary>
    /// 索引编写器
    /// </summary>
    public class Indexer : IElementIndexer
    {

      private ElementIdentityIndex _index;

      private Indexer( ElementIdentityIndex index )
      {
        _index = index;
      }

      void IElementIndexer.IndexElement( IHtmlElement element )
      {
        _index.IndexElement( element );
      }
    }



    private void IndexElement( IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();
      if ( id == null )
        return;


      if ( data.ContainsKey( id ) )
        throw new InvalidOperationException( string.Format( "文档结构不正确，存在两个 id 为 \"{0}\" 的元素，索引编制失败", id ) );


      data.Add( id, element );
    }


    private void RemoveElement( IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();
      if ( id == null )
        return;

      data.Remove( id );
    }




    void IDictionary<string, IHtmlElement>.Add( string key, IHtmlElement value )
    {
      throw new NotSupportedException();
    }

    bool IDictionary<string, IHtmlElement>.ContainsKey( string key )
    {
      return data.ContainsKey( key );
    }

    ICollection<string> IDictionary<string, IHtmlElement>.Keys
    {
      get { return data.Keys.ToArray(); }
    }

    bool IDictionary<string, IHtmlElement>.Remove( string key )
    {
      throw new NotSupportedException();
    }

    bool IDictionary<string, IHtmlElement>.TryGetValue( string key, out IHtmlElement value )
    {
      return data.TryGetValue( key, out value );
    }

    ICollection<IHtmlElement> IDictionary<string, IHtmlElement>.Values
    {
      get { return data.Values.ToArray(); }
    }

    IHtmlElement IDictionary<string, IHtmlElement>.this[string key]
    {
      get { return data[key]; }
      set { throw new NotSupportedException(); }
    }

    void ICollection<KeyValuePair<string, IHtmlElement>>.Add( KeyValuePair<string, IHtmlElement> item )
    {
      throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<string, IHtmlElement>>.Clear()
    {
      throw new NotSupportedException();
    }

    bool ICollection<KeyValuePair<string, IHtmlElement>>.Contains( KeyValuePair<string, IHtmlElement> item )
    {
      return data.Contains( item );
    }

    void ICollection<KeyValuePair<string, IHtmlElement>>.CopyTo( KeyValuePair<string, IHtmlElement>[] array, int arrayIndex )
    {
      throw new NotSupportedException();
    }

    int ICollection<KeyValuePair<string, IHtmlElement>>.Count
    {
      get { return data.Count; }
    }

    bool ICollection<KeyValuePair<string, IHtmlElement>>.IsReadOnly
    {
      get { return true; }
    }

    bool ICollection<KeyValuePair<string, IHtmlElement>>.Remove( KeyValuePair<string, IHtmlElement> item )
    {
      throw new NotSupportedException();
    }

    IEnumerator<KeyValuePair<string, IHtmlElement>> IEnumerable<KeyValuePair<string, IHtmlElement>>.GetEnumerator()
    {
      return data.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return data.GetEnumerator();
    }
  }
}
