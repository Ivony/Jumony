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


    private readonly HtmlCssSelector _selector;
    private readonly IEnumerable<IHtmlElement> _elements;

    public jQuery( string selector, IHtmlContainer scope )
    {
      _selectorExpression = selector;
      _scope = scope;

      _selector = HtmlCssSelector.Create( selector );
      _elements = _selector.Search( _scope, true );

    }




    public string attr( string name )
    {
      var element = _elements.FirstOrDefault();
      if ( element == null )
        return null;

      return element.Attribute( name ).Value();
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
      _elements.Select( element => element.Attribute( name ) ).NotNull().ForAll( attribute => attribute.Remove() );
      return this;
    }





    public string text()
    {
      var element = _elements.FirstOrDefault();
      if ( element == null )
        return null;

      return element.InnerText();
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
      var element = _elements.FirstOrDefault();
      if ( element == null )
        return null;

      return element.InnerHtml();
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



    public jQuery removeClass( string className )
    {
      return ForAll( element => element.Style().RemoveClass( className ) );
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
      return _elements.All( element => element.Style().Classes().Contains( className ) );
    }



    public string val()
    {
      var element = _elements.FirstOrDefault();
      if ( element == null )
        return null;

      return element.Attribute( "value" ).Value();
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


    protected IHtmlElement First
    {
      get { return _elements.FirstOrDefault(); }
    }



  }


  internal static class Extensions
  {
    public static IDictionary<string, string> ToPropertyDictionary( this object properties )
    {
      var dictionary = new Dictionary<string, string>();

      foreach ( PropertyDescriptor property in TypeDescriptor.GetProperties( properties ) )
      {
        var key =   property.Name;
        var _value = property.GetValue( properties );

        string value = null;

        if ( _value != null )
          value = _value.ToString();
      }

      return dictionary;
    }
  }
}
