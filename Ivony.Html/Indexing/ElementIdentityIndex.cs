using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{



  public interface IElementIndexer
  {
    void IndexElement( IHtmlElement element );
  }


  /// <summary>
  /// 文档的 ID 索引
  /// </summary>
  public class ElementIdentityIndex
  {


    private IHtmlDocument _document;

    internal ElementIdentityIndex( IHtmlDocument document )
    {
      _document = document;
    }


    private Dictionary<string, IHtmlElement> data = new Dictionary<string, IHtmlElement>();


    public void Rebuild()
    {
      data = new Dictionary<string, IHtmlElement>();

      _document.Descendants().ForAll( element => IndexElement( element ) );
    }


    public IHtmlElement this[string id]
    {
      get { return data[id]; }
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




  }
}
