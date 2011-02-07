using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html.Binding
{

  public static partial class HtmlBinder
  {

    private static readonly DefaultValueProvider defaultValueProvider = new DefaultValueProvider();

    public static IDictionary<string, object> GetValues( object data, IHtmlElement target )
    {

      return defaultValueProvider.GetValues( data, target );
    }



    public static IHtmlElement FindTarget( IHtmlElement element, string name, object value )
    {

      throw new NotImplementedException();

    }


    private static readonly DefaultElementBinder defaultElementBidner = new DefaultElementBinder();

    public static IElementBinder GetBinder( IHtmlElement target )
    {

      return new DefaultElementBinder();
    }
  }


  public interface ITargetProvider
  {

    IHtmlElement FindTarget( IHtmlElement targetElement, string name, object value );

  }

  public interface IValueProvider
  {

    IDictionary<string, object> GetValues( object data, IHtmlElement target );

  }





  public interface IUniformResource
  {
    Uri Locator
    {
      get;
    }
  }



  public interface IElementBinder
  {
    void Bind( object data, IHtmlElement element );
  }

  public class DefaultElementBinder : IElementBinder
  {
    #region IElementBinder 成员

    public void Bind( object data, IHtmlElement element )
    {
      var values = HtmlBinder.GetValues( data, element );

      foreach ( var key in values.Keys )
      {
        var value = values[key];

        var target = HtmlBinder.FindTarget( element, key, value );

        if ( target != null )
        {
          var binder = HtmlBinder.GetBinder( target );
          binder.Bind( value, element );
        }

      }
    }

    #endregion
  }

  public class DefaultValueProvider : IValueProvider
  {
    #region IValueProvider 成员

    public IDictionary<string, object> GetValues( object data, IHtmlElement target )
    {
      var properties = TypeDescriptor.GetProperties( data );

      return properties.Cast<PropertyDescriptor>().ToDictionary( p => p.Name, p => p.GetValue( data ) );
    }

    #endregion
  }



}
