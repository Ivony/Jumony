using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class HtmlDomDependency : IDisposable
  {

    internal HtmlDomDependency( IHtmlContainer container )
    {
      Container = container;

      AddDomChangedEventHandler( container );

    }

    private INotifyDomChanged FindDomChangedNotifier( IHtmlContainer container )
    {

      if ( container == null )
        throw new ArgumentNullException( "container" );

      var result = container as INotifyDomChanged;
      if ( result != null )
        return result;

      result = container.Document as INotifyDomChanged;
      if ( result != null )
        return null;


      return container.Document.DomModifier as INotifyDomChanged;

    }


    private void AddDomChangedEventHandler( IHtmlContainer container )
    {
      EventRaiser = container as INotifyDomChanged;
      if ( EventRaiser != null )
      {
        EventRaiser.HtmlDomChanged += DomChanged;
        return;
      }

      EventRaiser = container.Document as INotifyDomChanged;
      if ( EventRaiser != null )
      {
        EventRaiser.HtmlDomChanged += DomChanged;
        return;
      }


      EventRaiser = container.Document.DomModifier as INotifyDomChanged;
      if ( EventRaiser != null )
      {
        EventRaiser.HtmlDomChanged += DomChanged;
        return;
      }

      throw new NotSupportedException();
    }



    private INotifyDomChanged EventRaiser
    {
      get;
      set;
    }

    private void DomChanged( object sender, HtmlDomChangedEventArgs e )
    {
      HasChanged = true;
      EventRaiser.HtmlDomChanged -= DomChanged;
    }


    public IHtmlContainer Container
    {
      get;
      private set;
    }

    public bool HasChanged
    {
      get;
      private set;
    }



    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}
