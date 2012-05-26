using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    private void DataBind( IHtmlDomObject obj )
    {
      var bindings = FindBindings( obj );

      bindings.ForAll( b => b.DataBind() );

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



  }
}
