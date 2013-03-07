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
    /// <param name="scope">要监视 DOM 修改的范围</param>
    /// <param name="dependency">创建的依赖项</param>
    /// <returns>是否成功</returns>
    public static bool TryCreateDependency( IHtmlContainer scope, out HtmlDomDependency dependency )
    {
      return TryCreateDependency( scope, false, out dependency );
    }

    /// <summary>
    /// 创建一个 DOM 依赖项，当 DOM 结构发生更改时将会被标记为已过时。
    /// </summary>
    /// <param name="scope">要监视 DOM 修改的范围</param>
    /// <param name="inclusive">是否监视自身的修改</param>
    /// <param name="dependency">创建的依赖项</param>
    /// <returns>是否成功</returns>
    public static bool TryCreateDependency( IHtmlContainer scope, bool inclusive, out HtmlDomDependency dependency )
    {
      dependency = null;
      if ( scope == null )
        throw new ArgumentNullException( "container" );

      var notifier = scope.Document.DomModifier as INotifyDomChanged;
      if ( notifier == null )
        return false;

      if ( inclusive && !( scope is IHtmlNode ) )
        return false;


      dependency = new HtmlDomDependency();

      dependency.Notifier = notifier;
      dependency.ChangedDetermine = e =>
        {
          if ( e.Container.Equals( scope ) )
            return true;

          if ( inclusive && e.Node.Equals( scope ) )
            return true;

          var container = e.Container as IHtmlNode;

          if ( container != null && container.IsDescendantOf( scope ) )
            return true;

          return false;

        };

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
        if ( ChangedDetermine( e ) )
        {
          HasChanged = true;
          Notifier.HtmlDomChanged -= Handler;
        }
      }
    }

    private Predicate<HtmlDomChangedEventArgs> ChangedDetermine
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
