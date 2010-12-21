using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Ivony.Html
{
  public class AttributeSelector
  {
    public static readonly Regex attributeSelectorRegex = new Regex( Regulars.attributeExpressionPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant );


    private readonly string name;
    private readonly string separator;
    private readonly string value;

    private readonly string exp;


    private static readonly Regex whiteSpaceSeparatorRegex = new Regex( @"\p{Zs}", RegexOptions.Compiled | RegexOptions.CultureInvariant );


    private delegate bool ValueMatcher( string exp, string value );

    private static readonly Dictionary<string, ValueMatcher> matchers = new Dictionary<string, ValueMatcher>()
      {
        { "^=", ( exp, value ) => value != null && value.StartsWith( exp, StringComparison.Ordinal ) },
        { "$=", ( exp, value ) => value != null && value.EndsWith( exp, StringComparison.Ordinal ) },
        { "*=", ( exp, value ) => value != null && value.Contains( exp ) },
        { "~=", ( exp, value ) => value != null && whiteSpaceSeparatorRegex.Split( value ).Contains( exp,StringComparer.Ordinal ) },
        { "!=", ( exp, value ) => value != exp },
        { "=",  ( exp, value ) => value == exp }
      };





    public AttributeSelector( string expression )
    {

      exp = expression;

      var match = attributeSelectorRegex.Match( expression );

      if ( !match.Success )
        throw new FormatException();


      name = match.Groups["name"].Value;
      if ( match.Groups["separator"].Success )
      {
        separator = match.Groups["separator"].Value;
        if ( match.Groups["quoteText"].Success )
          value = match.Groups["quoteText"].Value;
        else
          value = match.Groups["value"].Value;
      }

    }


    public bool Allows( IHtmlElement element )
    {

      string _value = null;

      var attribute = element.Attribute( name );

      if ( separator == null )//如果没有运算符，那么表示判断属性是否存在
        return attribute != null;

      if ( attribute != null )
        _value = attribute.AttributeValue;

      return matchers[separator]( value, _value );
    }


    public override string ToString()
    {
      if ( separator != null )
        return string.Format( CultureInfo.InvariantCulture, "[{0}{1}'{2}']", name, separator, value.Replace( "'", "\\'" ).Replace( "\"", "\\\"" ) );
      else
        return string.Format( CultureInfo.InvariantCulture, "[{0}]", name );
    }

  }
}
