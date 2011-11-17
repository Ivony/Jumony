using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class HtmlDomDependency : IDisposable
  {



    private EventHandler<HtmlDomChangedEventArgs> Handler;

    private HtmlDomDependency()
    {
      Handler = new EventHandler<HtmlDomChangedEventArgs>( DomChanged );
    }


    /// <summary>
    /// 创建一个 DOM 依赖项，当 DOM 结构发生更改时将会被标记为已过时。
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static bool TryCreateDependency( IHtmlContainer container, out HtmlDomDependency dependency )
    {
      dependency = null;
      if ( container == null )
        throw new ArgumentNullException( "container" );

      var notifier = container.Document.DomModifier as INotifyDomChanged;
      if ( notifier == null )
        return false;

      dependency = new HtmlDomDependency();

      dependency.Notifier = notifier;
      dependency.Container = container;

      dependency.Notifier.HtmlDomChanged += dependency.Handler;

      return true;
    }


    private INotifyDomChanged Notifier
    {
      get;
      set;
    }


    private void DomChanged( object sender, HtmlDomChangedEventArgs e )
    {
      HasChanged = true;
      Notifier.HtmlDomChanged -= Handler;
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

    public void Reset()
    {
      HasChanged = false;
      Notifier.HtmlDomChanged += Handler;
    }



    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}
