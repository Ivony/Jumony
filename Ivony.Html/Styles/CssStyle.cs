using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Ivony.Fluent;


namespace Ivony.Html.Styles
{

  public static class StyleExtensions
  {
    public static CssStyle Style( this IHtmlElement element )
    {
      return new CssStyle( element );
    }
  }

  public class CssStyle
  {

    private static readonly Regex styleSettingsRegex = new Regex( string.Format( @"^\s*(?<styleSetting>{0})*$", Regulars.styleSettingPattern ), RegexOptions.Compiled );


    private readonly Hashtable settings = Hashtable.Synchronized( new Hashtable( StringComparer.InvariantCultureIgnoreCase ) );

    private IHtmlAttribute _attribute;

    internal CssStyle( IHtmlElement element )
    {
      element.SetAttribute( "style" );

      _attribute = element.Attribute( "style" );
      string styleValue = _attribute.Value().IfNull( "" );

      var match = styleSettingsRegex.Match( styleValue );

      if ( !match.Success )
        throw new FormatException();

      foreach ( var settingCapture in match.Groups["styleSetting"].Captures.Cast<Capture>() )
      {
        string name = settingCapture.FindCaptures( match.Groups["name"] ).Single().Value;
        string value = settingCapture.FindCaptures( match.Groups["value"] ).Single().Value;

        settings.Add( name, value );
      }
    }

    public string Get( string name )
    {
      return (string) settings[name];
    }

    public CssStyle Set( string name, string value )
    {
      settings[name] = value;

      RefreshAttribute();

      return this;
    }

    private void RefreshAttribute()
    {
      var builder = new StringBuilder();

      foreach ( DictionaryEntry entry in settings )
        builder.AppendFormat( "{0}: {1}; ", entry.Key, entry.Value );

      _attribute.Value( builder.ToString().Trim() );
    }

  }
}
