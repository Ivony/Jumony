using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{
  public class BindingManager
  {


    public IHtmlDocument Document
    {
      get;
      private set;
    }

    public BindingManager( IHtmlDocument document )
    {
      Document = document;

      var modifier = document.DomModifier as ISynchronizedDomModifier;
      if ( modifier == null )
        throw new NotSupportedException();

      SyncRoot = modifier.SyncRoot;
    }

    protected object SyncRoot
    {
      get;
      private set;
    }


    public void DataBind()
    {
      lock ( SyncRoot )
      {
        DataBind( Document );
      }
    }

    protected object DataContext
    {
      get;
      set;
    }


    protected virtual void DataBind( IHtmlDocument document )
    {
      var bindings = FindBindings( document );

      bindings.ForAll( b => b.DataBind( DataContext ) );

      document.Elements().ForAll( e => DataBind( e ) );
    }

    protected virtual void DataBind( IHtmlElement element )
    {
      var bindings = FindBindings( element );

      bindings.ForAll( b => b.DataBind( DataContext ) );

      element.Elements().ForAll( e => DataBind( e ) );

    }


    private IEnumerable<IBinding> FindBindings( IHtmlDocument document )
    {
      return Enumerable.Empty<IBinding>();
    }



    private IEnumerable<IBinding> FindBindings( IHtmlElement element )
    {

      return ElementBindingProviders.SelectMany( provider => provider.CreateBindings( this, element ) );

    }



    public IEnumerable<IBindingProvider> ElementBindingProviders
    {
      get;
      private set;
    }





    /// <summary>
    /// 创建默认的 binding 对象
    /// </summary>
    /// <param name="domObject">绑定的目标</param>
    /// <param name="args">绑定参数</param>
    /// <returns>Binding 对象</returns>
    protected virtual IBinding CreateDefaultBinding( IHtmlDomObject domObject, IDictionary<string, string> args )
    {

      return new Binding( this, domObject, args );

    }







    public IValueConverter GetConverter( string converterName )
    {
      throw new NotImplementedException();
    }

    public IValueBinder GetValueBinder( IHtmlDomObject TargetObject, object value )
    {
      throw new NotImplementedException();
    }


    public void GetValue( object dataContext, IDictionary<string, string> _bindingArgs )
    {
      throw new NotImplementedException();
    }

    internal static IValueBinder GetBinder( IHtmlDomObject BindingHost, object value )
    {
      throw new NotImplementedException();
    }

    internal static IBindingTarget GetTarget( IHtmlDomObject BindingHost, object value )
    {
      throw new NotImplementedException();
    }
  }


  public static class BindingExtensions
  {
    public static BindingManager BindingManager( this IHtmlDocument document )
    {
      return null;
    }


  }

}
