using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ivony.Fluent;
using System.Text.RegularExpressions;

namespace Ivony.Html.Indexing
{

  /// <summary>
  /// 样式类索引
  /// </summary>
  public class ElementClassIndex : ElementIndex
  {


    private object  _sync = new object();

    public ElementClassIndex( IHtmlDocument document )
      : base( document )
    {
    }

    private IDictionary<string,List<IHtmlElement>> data;



    protected override void InitializeData()
    {
      data = new Dictionary<string, List<IHtmlElement>>();
    }


    protected override void AddElement( IHtmlElement element )
    {
      var classes = element.Attribute( "class" ).Value();

      if ( !string.IsNullOrEmpty( classes ) )
        Regulars.whiteSpaceSeparatorRegex.Split( classes ).ForAll( c => AddElement( c, element ) );
    }



    private void AddElement( string className, IHtmlElement element )
    {
      var set = data[className] as List<IHtmlElement>;
      if ( set == null )
        set = data[className] = new List<IHtmlElement>();

      set.Add( element );
    }




    protected override void RemoveElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }
  }
}
