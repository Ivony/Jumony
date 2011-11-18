using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{


  /// <summary>
  /// HTML DOM 结构依赖项
  /// </summary>
  public class HtmlDomDependency : IDisposable
  {



    private EventHandler<HtmlDomChangedEventArgs> Handler;

    private HtmlDomDependency()
    {
      Handler = new EventHandler<HtmlDomChangedEventArgs>( DomChanged );
    }


    private object _sync = new object();

    private bool _disposed = false;


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
      lock ( _sync )
      {
        if ( e.Node.IsDescendantOf( Container ) )
        {
          HasChanged = true;
          Notifier.HtmlDomChanged -= Handler;
        }
      }
    }


    private IHtmlContainer Container
    {
      get;
      set;
    }

    /// <summary>
    /// 自创建或上次重置以来 DOM 结构是否已被更改
    /// </summary>
    public bool HasChanged
    {
      get;
      private set;
    }



    /// <summary>
    /// 重置修改状态
    /// </summary>
    public void Reset()
    {
      lock ( _sync )
      {
        if ( _disposed )
          throw new ObjectDisposedException( "DomDependency" );

        HasChanged = false;
        Notifier.HtmlDomChanged += Handler;
      }
    }



    /// <summary>
    /// 销毁依赖项
    /// </summary>
    public void Dispose()
    {
      lock ( _sync )
      {

        if ( _disposed )
          return;

        if ( !HasChanged )
          Notifier.HtmlDomChanged -= Handler;
        _disposed = true;
      }
    }
  }
}
