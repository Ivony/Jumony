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
  public class ElementNameIndex : ElementIndex
  {


    private IHtmlDocument _document;

    /// <summary>
    /// 创建元素名称的索引
    /// </summary>
    /// <param name="document"></param>
    public ElementNameIndex( IHtmlDocument document ) : base( document ) { }


    private IDictionary<string,List<IHtmlElement>> data;



    protected override void InitializeData()
    {
      data = new Dictionary<string, List<IHtmlElement>>();
    }




    protected override void AddElement( IHtmlElement element )
    {
      var name = element.Name;

      AddElement( name, element );
    }


    private void AddElement( string name, IHtmlElement element )
    {
      var set = data[name] as List<IHtmlElement>;
      if ( set == null )
        set = data[name] = new List<IHtmlElement>();

      set.Add( element );
    }



    protected override void RemoveElement( IHtmlElement element )
    {
      var set = data[element.Name] as List<IHtmlElement>;
      set.Remove( element );
    }


    protected override void UpdateElement( IHtmlElement element )
    {
    }

  }
}
