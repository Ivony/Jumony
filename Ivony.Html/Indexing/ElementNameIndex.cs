using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html.Indexing
{

  /// <summary>
  /// 元素名称的索引
  /// </summary>
  public class ElementNameIndex
  {


    private IHtmlDocument _document;

    /// <summary>
    /// 创建元素名称的索引
    /// </summary>
    /// <param name="document"></param>
    public ElementNameIndex( IHtmlDocument document )
    {
      _document = document;
    }


    private IDictionary<string,List<IHtmlElement>> data;


    public void Rebuild()
    {
      data = new Dictionary<string, List<IHtmlElement>>();

      _document.Descendants().ForAll( element => IndexElement( element ) );

    }

    private void IndexElement( IHtmlElement element )
    {
      var name = element.Name;

      IndexElement( name, element );

    }



    private void IndexElement( string name, IHtmlElement element )
    {
      var set = data[name] as List<IHtmlElement>;
      if ( set == null )
        set = data[name] = new List<IHtmlElement>();

      set.Add( element );
    }

  }
}
