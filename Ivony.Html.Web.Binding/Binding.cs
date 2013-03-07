using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Web.Binding
{


  public interface IBinding
  {
    void DataBind( BindingContext context );

    int Priority { get; }
  }


  public class Binding : IBinding
  {
    private IDictionary<string, string> _arguments;

    public Binding( IHtmlDomObject bindingHost, IDictionary<string, string> args )
    {
      BindingHost = bindingHost;
      _arguments = args;
    }


    public IHtmlDomObject BindingHost
    {
      get;
      private set;
    }


    public void DataBind( BindingContext context )
    {

      object dataObject = null;

      var dataContext = context.DataContext;


      if ( dataContext != null )
      {
        string path;

        if ( _arguments.TryGetValue( "path", out path ) )
          dataObject = Eval( dataContext, path );

        else
          dataObject = dataContext;
      }


      string converterName;
      if ( !_arguments.TryGetValue( "converter", out converterName ) )
        converterName = null;

      string binderName;
      if ( _arguments.TryGetValue( "binder", out binderName ) )
      {
        var valueBinder = context.BindingManager.GetValueBinder( BindingHost, binderName );

        if ( valueBinder != null )
        {

          var value = context.BindingManager.ConvertValue( dataObject, valueBinder.ValueType, converterName );

          valueBinder.BindValue( BindingHost, value );
          return;
        }
      }



      {
        var target = context.BindingManager.GetTarget( BindingHost );

        var value = context.BindingManager.ConvertValue( dataContext, target.ValueType, converterName );
        target.BindValue( value );
      }

    }



    protected virtual object Eval( object dataContext, string path )
    {
      return DataBinder.Eval( dataContext, path );
    }



    protected virtual object ConvertValue( BindingContext context, object dataObject, Type targetType )
    {
      var converter = context.BindingManager.GetConverter( _arguments["converter"], targetType );

      if ( converter != null )
        return converter.Convert( dataObject );

      return dataObject;

    }



    public int Priority
    {
      get { return 10000; }
    }
  }
}
