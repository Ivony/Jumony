using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{




  /// <summary>
  /// 文档的 ID 索引
  /// </summary>
  public class ElementIdentityIndex : ElementIndex
  {


    public ElementIdentityIndex( IHtmlDocument document ) : base( document ) { }


    private Dictionary<string, IHtmlElement> data = new Dictionary<string, IHtmlElement>();



    protected override void InitializeData()
    {
      data = new Dictionary<string, IHtmlElement>();
    }


    public IHtmlElement this[string id]
    {
      get { return data[id]; }
    }


    protected override void AddElement( IHtmlElement element )
    {
      var id = element.Attribute( "id" ).Value();
      if ( id == null )
        return;


      if ( data.ContainsKey( id ) )
        throw new InvalidOperationException( string.Format( "文档结构不正确，存在两个 id 为 \"{0}\" 的元素，索引编制失败", id ) );


      data.Add( id, element );
    }


    protected override void RemoveElement( IHtmlElement element )
    {
      RemoveElement( element, element.Attribute( "id" ) );
    }

    private void RemoveElement( IHtmlElement element, IHtmlAttribute attribute )
    {
      var id = attribute.Value();
      if ( id == null )
        return;

      data.Remove( id );
    }




    protected override void OnAddAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      AddElement( element );
    }

    protected override void OnRemoveAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      RemoveElement( element, attribute );
    }
  }
}
