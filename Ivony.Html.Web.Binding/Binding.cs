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

    IHtmlNode Target { get; }

    int Priority { get; }
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
      var valueBinder = BindingManager.GetBinder( BindingHost, value );

      if ( valueBinder != null )
      {
        valueBinder.BindValue( BindingHost, value );
        return;
      }

      var target = BindingManager.GetTarget( BindingHost, value );
      target.BindValue( value );

    }




    public IHtmlNode Target
    {
      get { throw new NotImplementedException(); }
    }

    public int Priority
    {
      get { throw new NotImplementedException(); }
    }
  }
}
