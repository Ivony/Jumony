using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Ivony.Fluent;

namespace Ivony.Html
{


  /// <summary>
  /// 代表一个元素选择器
  /// </summary>
  public class CssElementSelector
  {
    /// <summary>
    /// 匹配CSS元素选择器的正则表达式。
    /// </summary>
    public static readonly Regex elementSelectorRegex = new Regex( Regulars.elementExpressionPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );

    public CssElementSelector( string expression )
    {

      if ( string.IsNullOrEmpty( expression ) )
        throw new ArgumentNullException( "expression" );

      var match = elementSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException( string.Format( CultureInfo.InvariantCulture, "元素选择器 \"{0}\" 格式不正确", expression ) );


      if ( match.Groups["name"].Success )
        _tagName = match.Groups["name"].Value;
      else
        _tagName = "*";

      var _attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new CssAttributeSelector( c.Value ) ).ToList();

      if ( match.Groups["identity"].Success )
        _attributeSelectors.Add( new CssAttributeSelector( string.Format( CultureInfo.InvariantCulture, "[id={0}]", match.Groups["identity"].Value ) ) );

      if ( match.Groups["class"].Success )
        _attributeSelectors.Add( new CssAttributeSelector( string.Format( CultureInfo.InvariantCulture, "[class~={0}]", match.Groups["class"].Value ) ) );

      attributeSelectors = _attributeSelectors.ToArray();

      pseudoClassSelectors = match.Groups["pseudoClassSelector"].Captures.Cast<Capture>().Select( c => CssPseudoClassSelectors.Create( c.Value ) ).ToArray();

    }


    private string _tagName;

    private readonly CssAttributeSelector[] attributeSelectors;

    private readonly ICssPseudoClassSelector[] pseudoClassSelectors;


    public bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;


      if ( _tagName != "*" && !element.Name.EqualsIgnoreCase( _tagName ) )
        return false;

      foreach ( var selector in attributeSelectors )
      {
        if ( !selector.IsEligible( element ) )
          return false;
      }

      foreach ( var selector in pseudoClassSelectors )
      {
        if ( !selector.Allows( element ) )
          return false;
      }

      return true;
    }


    public IEnumerable<IHtmlElement> Search( IEnumerable<IHtmlElement> source )
    {
      return source.Where( element => IsEligible( element ) );
    }


    public override string ToString()
    {
      return string.Format( "{0}{1}{2}", _tagName.ToUpper(), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ), string.Join( "", pseudoClassSelectors.Select( p => p.ToString() ).ToArray() ) );
    }


    public string ElementName { get { return _tagName; } }



  }
}
