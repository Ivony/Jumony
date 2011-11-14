using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class DomDependency : IDisposable
  {

    internal DomDependency( IHtmlContainer container )
    {
      Container = container;

      AddDomChangedEventHandler( container );

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
