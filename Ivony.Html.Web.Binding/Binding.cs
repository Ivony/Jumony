using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Web.Binding
{


  public interface IBinding
  {
    void DataBind( object dataContext );
  }


  public class Binding : IBinding
  {
    private IDictionary<string, string> args;

    public Binding( BindingManager manager, IHtmlDomObject domObject, IDictionary<string, string> args )
    {
      BindingManager = manager;
      BindingHost = domObject;
      this.args = args;
    }

    public IHtmlDomObject BindingHost
    {
      get;
      private set;
    }


    public BindingManager BindingManager
    {
      get;
      private set;
    }



    public void DataBind( object dataContext )
    {

      object dataObject = null;

      if ( dataContext != null )
      {
        var path = args["path"];

        if ( path == null )
          dataObject = dataContext;

        else
          dataObject = Eval( dataContext, path );

      }

      object value = ConvertValue( dataObject );

      BindValue( value );

    }



    protected virtual object Eval( object dataContext, string path )
    {
      return DataBinder.Eval( dataContext, path );
    }



    protected virtual object ConvertValue( object dataObject )
    {
      var converter = BindingManager.GetConverter( args["converter"] );

      if ( converter != null )
        return converter.Convert( dataObject );

      return dataObject;

    }


    protected virtual void BindValue( object value )
    {
      var valueBinder = BindingManager.GetValueBinder( BindingHost, value );

      if ( valueBinder == null )
        DefaultBindValue( value );

      else
        valueBinder.BindValue( BindingHost, value );

    }

    protected virtual void DefaultBindValue( object value )
    {
      var attribute = BindingHost as IHtmlAttribute;

      if ( attribute != null )
        BindAttributeValue( attribute, value );

    }

    protected virtual void BindAttributeValue( IHtmlAttribute attribute, object value )
    {
      if ( value == null )
      {
        attribute.Remove();
        return;
      }


      if ( value is bool )
      {
        if ( (bool) value )
          attribute.SetValue( null );
        else
          attribute.Remove();
      }


      attribute.SetValue( value.ToString() );
    }

    protected virtual void BindElementValue( IHtmlElement element, object value )
    {

      throw new NotImplementedException();

    }




  }
}
