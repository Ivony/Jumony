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


    private void DataBind( IHtmlDomObject obj )
    {
      var bindings = FindBindings( obj );

      bindings.ForAll( b => b.DataBind( DataContext ) );

      var element = obj as IHtmlElement;

      if ( element != null )
        element.Attributes().ForAll( a => DataBind( a ) );

      var container = obj as IHtmlContainer;

      if ( container != null )
        container.Nodes().ForAll( n => DataBind( n ) );
    }

    private IEnumerable<IBinding> FindBindings( IHtmlDomObject obj )
    {
      return Enumerable.Empty<IBinding>();
    }



    public IEnumerable<IBindingProvider> BindingProviders
    {
      get;
      private set;
    }



    /// <summary>
    /// 创建 Binding 对象
    /// </summary>
    /// <param name="domObject">绑定的目标</param>
    /// <param name="args">绑定参数</param>
    /// <returns>Binding 对象</returns>
    public IBinding CreateBinding( IHtmlDomObject domObject, IDictionary<string, string> args )
    {
      foreach ( var provider in BindingProviders )
      {
        var binding = provider.CreateBinding( this, domObject, args );
        if ( binding != null )
          return binding;
      }

      return CreateDefaultBinding( domObject, args );
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
  }


  public static class BindingExtensions
  {
    public static BindingManager BindingManager( this IHtmlDocument document )
    {
      return null;
    }


  }

}
