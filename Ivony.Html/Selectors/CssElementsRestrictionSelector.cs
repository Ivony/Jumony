using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class CssElementsRestrictionSelector : ICssSelector
  {

    private readonly HashSet<IHtmlElement> _elements;

    public CssElementsRestrictionSelector( IEnumerable<IHtmlElement> elements )
    {

      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      _elements = new HashSet<IHtmlElement>( elements );

    }

    public bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;

      return _elements.Contains( element );
    }

    public override string ToString()
    {
      return "#elements#";
    }
  }
}
