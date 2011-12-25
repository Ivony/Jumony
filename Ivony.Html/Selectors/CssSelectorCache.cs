using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;

namespace Ivony.Html.Selectors
{
  public class CssSelectorIndex
  {

    private IHtmlDocument _document;

    internal CssSelectorIndex( IHtmlDocument document )
    {
      _document = document;
    }



    public CssSelectorIdentityIndex IdentityIndex
    {
      get;
      private set;
    }


    public void Rebuild( bool indexIdentity, bool indexClass, bool indexElementName, bool indexAttribute )
    {

      List<IElementIndexer> indexers = new List<IElementIndexer>();

      if ( indexIdentity )
      {
        IdentityIndex = new CssSelectorIdentityIndex( this );
        indexers.Add( IdentityIndex );
      }

      Rebuild( indexers );
    }

    private void Rebuild( List<IElementIndexer> indexers )
    {
      _document.Descendants().ForAll( element => indexers.ForAll( indexer => indexer.IndexElement( element ) ) );
    }


  }



  public interface IElementIndexer
  {
    void IndexElement( IHtmlElement element );
  }


  /// <summary>
  /// 文档的 ID 索引
  /// </summary>
  public class CssSelectorIdentityIndex : IDictionary<string, IHtmlElement>, IElementIndexer
  {
    internal CssSelectorIdentityIndex( CssSelectorIndex index )
    {

    }


    private Dictionary<string, IHtmlElement> data = new Dictionary<string, IHtmlElement>();


    void IElementIndexer.IndexElement( IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();
      if ( id == null )
        return;


      if ( data.ContainsKey( id ) )
        throw new InvalidOperationException( string.Format( "文档结构不正确，存在两个 id 为 \"{0}\" 的元素，索引编制失败", id ) );


      data.Add( id, element );
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
      return data.CastTo<IDictionary<string, IHtmlElement>>().Contains( item );
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

#if false

  /// <summary>
  /// 文档 class 样式的索引
  /// </summary>
  public class CssSelectorClassIndex : IDictionary<string, IEnumerable<IHtmlElement>>, IElementIndexer
  {

  }

#endif
}
