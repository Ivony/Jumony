using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;
using System.ComponentModel;
using Ivony.Html.Styles;


namespace Ivony.Html
{
  public class jQuery
  {


    private readonly string _selectorExpression;
    private readonly IHtmlContainer _scope;


    private readonly ICssSelector _selector;
    private readonly IEnumerable<IHtmlElement> _elements;

    public jQuery( string selector, IHtmlContainer scope )
    {
      _selectorExpression = selector;
      _scope = scope;

      _selector = CssSelector.Create( selector, scope );
      _elements = _selector.Filter( _scope.Descendants() );

    }







    public string attr( string name )
    {
      return ForFirstOrNull( e => e.Attribute( name ).Value() );
    }

    public jQuery attr( string name, string value )
    {
      _elements.SetAttribute( name ).Value( value );
      return this;
    }

    public jQuery attr( string name, Func<string, string> evaluator )
    {
      _elements.SetAttribute( name ).Value( evaluator );
      return this;
    }

    public jQuery attr( string name, Func<int, string, string> evaluator )
    {
      _elements.SetAttribute( name ).Value( evaluator );
      return this;
    }


    public jQuery attr( object properties )
    {
      return attr( properties.ToPropertyDictionary() );
    }


    public jQuery attr( IDictionary<string, string> properties )
    {
      var elements = _elements.ToArray();

      foreach ( var pair in properties )
        elements.SetAttribute( pair.Key ).Value( pair.Value );

      return this;
    }


    public jQuery removeAttr( string name )
    {
      ForAll( e => e.SetAttribute( name ).Remove() );
      return this;
    }





    public string text()
    {
      return ForFirstOrNull( e => e.InnerText() );
    }

    public jQuery text( string text )
    {
      ForAll( element => element.InnerText( text ) );
      return this;
    }

    public jQuery text( Func<string, string> evaluator )
    {
      ForAll( element => element.InnerText( evaluator( element.InnerText() ) ) );
      return this;
    }

    public jQuery text( Func<int, string, string> evaluator )
    {
      ForAll( ( element, i ) => element.InnerText( evaluator( i, element.InnerText() ) ) );
      return this;
    }




    public string html()
    {
      return ForFirstOrNull( e => e.InnerHtml() );
    }

    public jQuery html( string html )
    {
      ForAll( element => element.InnerHtml( html ) );
      return this;
    }

    public jQuery html( Func<string, string> evaluator )
    {
      ForAll( element => element.InnerHtml( evaluator( element.InnerHtml() ) ) );
      return this;
    }

    public jQuery html( Func<int, string, string> evaluator )
    {
      ForAll( ( element, i ) => element.InnerHtml( evaluator( i, element.InnerHtml() ) ) );
      return this;
    }



    public jQuery addClass( string className )
    {
      return ForAll( element => element.Style().AddClass( className ) );
    }

    public jQuery addClass( Func<int, string, string> classEvaluator )
    {
      return addClass( ( i, classes ) => classEvaluator( i, string.Join( " ", classes.ToArray() ) ).Split( ' ' ) );
    }

    public jQuery addClass( Func<int, IEnumerable<string>, IEnumerable<string>> classEvaluator )
    {
      return ForAll( ( e, i ) => classEvaluator( i, e.Style().Classes() ).ForAll( c => e.Style().AddClass( c ) ) );
    }




    public jQuery removeClass( string className )
    {
      return ForAll( element => element.Style().RemoveClass( className ) );
    }

    public jQuery removeClass( Func<int, string, string> classEvaluator )
    {
      return removeClass( ( i, classes ) => classEvaluator( i, string.Join( " ", classes.ToArray() ) ).Split( ' ' ) );
    }

    public jQuery removeClass( Func<int, IEnumerable<string>, IEnumerable<string>> classEvaluator )
    {
      return ForAll( ( e, i ) => classEvaluator( i, e.Style().Classes() ).ForAll( c => e.Style().RemoveClass( c ) ) );
    }



    public jQuery toogleClass( string className )
    {
      return ForAll( element =>
        {

          lock ( element.SyncRoot )
          {
            if ( element.Style().Classes().Contains( className ) )
              element.Style().RemoveClass( className );
            else
              element.Style().AddClass( className );
          }

        } );
    }



    public bool hasClass( string className )
    {
      return _elements.Any( element => element.Style().Classes().Contains( className ) );
    }



    public string val()
    {
      return attr( "value" );
    }

    public jQuery val( string value )
    {
      return attr( "value", value );
    }

    public jQuery val( Func<int, string, string> valueEvaluator )
    {
      return attr( "value", valueEvaluator );
    }




    public string css( string propertyName )
    {
      return ForFirstOrNull( e => e.Style().Get( propertyName ) );
    }

    public jQuery css( string propertyName, string value )
    {
      return ForAll( e => e.Style().Set( propertyName, value ) );
    }

    public jQuery css( string propertyName, Func<int, string, string> evaluator )
    {
      return ForAll( ( e, i ) => e.Style().Set( propertyName, evaluator( i, e.Style().Get( propertyName ) ) ) );
    }

    public jQuery css( object map )
    {
      return css( map.ToPropertyDictionary() );
    }

    public jQuery css( IDictionary<string, string> map )
    {
      foreach ( var e in _elements.ToArray() )
      {
        lock ( e.SyncRoot )
        {
          foreach ( var pair in map )
          {
            e.Style().Set( pair.Key, pair.Value );
          }
        }
      }

      return this;
    }















    public jQuery each( Action<int, IHtmlElement> action )
    {
      _elements.ForAll( ( e, i ) => action( i, e ) );
      return this;
    }



    protected jQuery ForAll( Action<IHtmlElement> action )
    {
      _elements.ForAll( action );
      return this;
    }

    protected jQuery ForAll( Action<IHtmlElement, int> action )
    {
      _elements.ForAll( action );
      return this;
    }


    protected T ForFirstOrNull<T>( Func<IHtmlElement, T> evaluator ) where T : class
    {
      var first = _elements.FirstOrDefault();

      if ( first == null )
        return null;

      return evaluator( first );
    }


  }


  public static class Extensions
  {
    internal static IDictionary<string, string> ToPropertyDictionary( this object properties )
    {
      var dictionary = new Dictionary<string, string>();

      foreach ( PropertyDescriptor property in TypeDescriptor.GetProperties( properties ) )
      {
        var key = property.Name;
        var _value = property.GetValue( properties );

        string value = null;

        if ( _value != null )
          value = _value.ToString();
      }

      return dictionary;
    }




  }
}
