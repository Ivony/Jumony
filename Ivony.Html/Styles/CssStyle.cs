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

    public static CssStyle Style( this IEnumerable<IHtmlElement> elements )
    {
      return new CssStyleSetSetter( elements );
    }
  }



  public class CssStyle
  {

    private static readonly Regex styleSettingsRegex = new Regex( string.Format( @"^\s*(?<styleSetting>{0})*$", Regulars.styleSettingPattern ), RegexOptions.Compiled );


    private readonly Hashtable settings = Hashtable.Synchronized( new Hashtable( StringComparer.InvariantCultureIgnoreCase ) );

    private IHtmlElement _element;

    internal CssStyle()
    {
    }

    internal CssStyle( IHtmlElement element )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      _element = element;

      lock ( element.SyncRoot )
      {
        settings = GetStyleSettings( element.Attribute( "style" ).Value() );
      }
    }

    public virtual string Get( string name )
    {
      return (string) settings[name];
    }

    public virtual CssStyle Set( string name, string value )
    {
      settings[name] = value;

      _element.SetAttribute( "style" ).Value( GetStyleExpression( settings ) );

      return this;
    }




    protected Hashtable GetStyleSettings( string styleExpression )
    {

      if ( string.IsNullOrEmpty( styleExpression ) )
        return new Hashtable();

      var match = styleSettingsRegex.Match( styleExpression );

      if ( !match.Success )
        throw new FormatException();

      foreach ( var settingCapture in match.Groups["styleSetting"].Captures.Cast<Capture>() )
      {
        string name = settingCapture.FindCaptures( match.Groups["name"] ).Single().Value;
        string value = settingCapture.FindCaptures( match.Groups["value"] ).Single().Value;

        settings.Add( name, value );
      }

      return settings;
    }




    private string GetStyleExpression( Hashtable styleSettings )
    {
      var builder = new StringBuilder();

      foreach ( DictionaryEntry entry in styleSettings )
        builder.AppendFormat( "{0}: {1}; ", entry.Key, entry.Value );

      return builder.ToString().Trim();
    }



    public IHtmlElement AddClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( !classes.Contains( className ) )
          classes.Add( className );

        SetClasses( classes );
      }

      return _element;
    }


    public IHtmlElement RemoveClass( string className )
    {
      lock ( _element.SyncRoot )
      {
        var classes = GetClasses();

        if ( classes.Contains( className ) )
          classes.Remove( className );

        SetClasses( classes );
      }

      return _element;
    }


    public IEnumerable<string> Classes()
    {
      return GetClasses();
    }



    private void SetClasses( HashSet<string> classes )
    {
      _element.SetAttribute( "class", string.Join( " ", classes.ToArray() ) );
    }

    private HashSet<string> GetClasses()
    {
      var classSet = new HashSet<string>();


      var classes = _element.Attribute( "class" ).Value();
      if ( classes == null )
        return classSet;

      foreach ( var c in classes.Split( ' ' ) )
      {
        if ( string.IsNullOrEmpty( c ) )
          continue;

        if ( classSet.Contains( c ) )
          continue;


        classSet.Add( c );
      }

      return classSet;
    }



  }



  internal class CssStyleSetSetter : CssStyle
  {

    private IEnumerable<IHtmlElement>  _elements;

    public CssStyleSetSetter( IEnumerable<IHtmlElement> elements )
    {
      _elements = elements;
    }

    public override string Get( string name )
    {
      throw new NotSupportedException( "CssStyle 对象在表示元素集的时候，不支持 Get 方法。" );
    }

    public override CssStyle Set( string name, string value )
    {
      _elements.ForAll( e => e.Style().Set( name, value ) );

      return this;
    }
  }
}
