using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ivony.Fluent;
using System.Text.RegularExpressions;

namespace Ivony.Html.Indexing
{
  public class ElementClassIndex
  {


    private object  _sync = new object();

    private IHtmlDocument _document;

    public ElementClassIndex( IHtmlDocument document )
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
      var classes = element.Attribute( "class" ).Value();

      if ( !string.IsNullOrEmpty( classes ) )
        Regulars.whiteSpaceSeparatorRegex.Split( classes ).ForAll( c => IndexElement( c, element ) );
    }



    private void IndexElement( string className, IHtmlElement element )
    {
      var set = data[className] as List<IHtmlElement>;
      if ( set == null )
        set = data[className] = new List<IHtmlElement>();

      set.Add( element );
    }



  }
}
