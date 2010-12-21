using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Ivony.Fluent;

namespace Ivony.Html
{
  public class ElementSelector
  {

    public static readonly Regex elementSelectorRegex = new Regex( Regulars.elementExpressionPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );

    public ElementSelector( string expression )
    {

      if ( string.IsNullOrEmpty( expression ) )
        throw new FormatException();

      var match = elementSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      if ( match.Groups["name"].Success )
        _tagName = match.Groups["name"].Value;
      else
        _tagName = "*";

      var _attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new AttributeSelector( c.Value ) ).ToList();

      if ( match.Groups["identity"].Success )
        _attributeSelectors.Add( new AttributeSelector( string.Format( CultureInfo.InvariantCulture, "[id={0}]", match.Groups["identity"].Value ) ) );

      if ( match.Groups["class"].Success )
        _attributeSelectors.Add( new AttributeSelector( string.Format( CultureInfo.InvariantCulture, "[class~={0}]", match.Groups["class"].Value ) ) );

      attributeSelectors = _attributeSelectors.ToArray();

      pseudoClassSelectors = match.Groups["pseudoClassSelector"].Captures.Cast<Capture>().Select( c => PseudoClassFactory.Create( c.Value ) ).ToArray();

    }


    private string _tagName;

    private readonly AttributeSelector[] attributeSelectors;

    private readonly IPseudoClassSelector[] pseudoClassSelectors;


    public IEnumerable<IHtmlElement> Filter( IEnumerable<IHtmlElement> source )
    {
      return source.Where( item => Allows( item ) );
    }

    public bool Allows( IHtmlElement element )
    {
      if ( element == null )
        return false;


      if ( _tagName != "*" && !element.Name.EqualsIgnoreCase( _tagName ) )
        return false;

      foreach ( var selector in attributeSelectors )
      {
        if ( !selector.Allows( element ) )
          return false;
      }

      foreach ( var selector in pseudoClassSelectors )
      {
        if ( !selector.Allows( this, element ) )
          return false;
      }

      return true;
    }


    public IEnumerable<IHtmlElement> Search( IEnumerable<IHtmlElement> elements )
    {
      return elements.Where( e => Allows( e ) );
    }


    public override string ToString()
    {
      return string.Format( "{0}{1}{2}", _tagName.ToUpper(), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ), string.Join( "", pseudoClassSelectors.Select( p => p.ToString() ).ToArray() ) );
    }


    public string TagName { get { return _tagName; } }
  }
}
