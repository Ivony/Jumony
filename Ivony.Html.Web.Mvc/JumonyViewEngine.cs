using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Compilation;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 基于 Jumony 技术的视图引擎
  /// </summary>
  public class JumonyViewEngine : VirtualPathProviderViewEngine
  {

    /// <summary>
    /// 创建 JumonyViewEngine 对象
    /// </summary>
    public JumonyViewEngine()
    {

      ViewLocationFormats = new[]
      {
        "~/Views/{1}/{0}.htm",
        "~/Views/{1}/{0}.html",
        "~/Views/Shared/{0}.htm",
        "~/Views/Shared/{0}.html"
      };

      AreaViewLocationFormats = new[]
      {
        "~/Areas/{2}/Views/{1}/{0}.htm",
        "~/Areas/{2}/Views/{1}/{0}.html",
        "~/Areas/{2}/Views/Shared/{0}.htm",
        "~/Areas/{2}/Views/Shared/{0}.html",
      };

      PartialViewLocationFormats = ViewLocationFormats;
      AreaPartialViewLocationFormats = AreaViewLocationFormats;

      MasterLocationFormats = ViewLocationFormats;
      AreaMasterLocationFormats = AreaMasterLocationFormats;

      ViewLocationCache = new JumonyViewLocationCache();
    }








    /// <summary>
    /// 创建部分视图
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <param name="partialPath"></param>
    /// <returns></returns>
    protected override IView CreatePartialView( ControllerContext controllerContext, string partialPath )
    {
      return CreateViewCore( controllerContext, partialPath, true );
    }


    /// <summary>
    /// 创建页面视图
    /// </summary>
    /// <param name="controllerContext">控制器上下文</param>
    /// <param name="viewPath">视图路径</param>
    /// <param name="masterPath">母板路径</param>
    /// <exception cref="System.NotSupportedException">当 masterPath 参数不为空，且当前创建的视图不支持母板时引发</exception>
    /// <returns>页面视图对象</returns>
    protected override IView CreateView( ControllerContext controllerContext, string viewPath, string masterPath )
    {
      var view = CreateViewCore( controllerContext, viewPath, false );

      if ( string.IsNullOrEmpty( masterPath ) )
      {

        var directory = VirtualPathUtility.GetDirectory( viewPath );

        do
        {
          masterPath = VirtualPathUtility.Combine( directory, "_master.html" );

          if ( VirtualPathProvider.FileExists( masterPath ) )
          {
            var contentView = view as IContentView;

            if ( contentView != null )
            {

              contentView.InitializeMaster( CreateMaster( controllerContext, masterPath ) );
              return contentView;

            }
          }

          if ( directory == "~/" )
            break;

          directory = VirtualPathUtility.Combine( directory, "../" );

        } while ( MvcEnvironment.Configuration.FallbackDefaultMaster );

        return view;
      }

      else
      {
        var contentView = view as IContentView;

        if ( contentView == null )
          throw new InvalidOperationException( "视图不支持母板" );

        contentView.InitializeMaster( CreateMaster( controllerContext, masterPath ) );

        return contentView;
      }
    }


    /// <summary>
    /// 创建视图母板
    /// </summary>
    /// <param name="controllerContext">控制器上下文</param>
    /// <param name="masterPath">母板路径</param>
    /// <returns>创建的视图母板</returns>
    protected virtual JumonyMasterView CreateMaster( ControllerContext controllerContext, string masterPath )
    {
      var handlerPath = masterPath + ".ashx";
      JumonyMasterView masterView = null;

      if ( VirtualPathProvider.FileExists( handlerPath ) )
        masterView = (MasterViewHandler) BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( MasterViewHandler ) );

      if ( masterView == null )
        masterView = new JumonyMasterView();

      masterView.Initialize( masterPath );
      return masterView;
    }



    /// <summary>
    /// 创建视图对象
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <param name="isPartial">是否为部分视图</param>
    /// <returns>视图对象</returns>
    protected virtual IView CreateViewCore( ControllerContext context, string virtualPath, bool isPartial )
    {
      IViewProvider viewProvider;
      var view = CreateViewCore( context, virtualPath, isPartial, out viewProvider );
      view.Initialize( virtualPath, isPartial );
      OnViewCreated( new JumonyViewEventArgs() { View = view, ViewProvider = viewProvider } );
      return view;
    }


    /// <summary>
    /// 创建视图对象
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <param name="isPartial">是否为部分视图</param>
    /// <param name="viewProvider">产生该视图的视图提供程序</param>
    /// <returns>视图对象</returns>
    protected virtual ViewBase CreateViewCore( ControllerContext context, string virtualPath, bool isPartial, out IViewProvider viewProvider )
    {

      lock ( _providersSync )
      {
        foreach ( var provider in ViewProviders )
        {
          var view = provider.TryCreateView( context, VirtualPathProvider, virtualPath, isPartial );
          if ( view != null )
          {
            viewProvider = provider;
            return view;
          }
        }
      }



      {//默认处理策略

        viewProvider = null;
        ViewBase view = TryCreateViewHandler( virtualPath, isPartial );


        if ( view == null )
        {
          view = new JumonyView();
        }


        OnViewCreated( new JumonyViewEventArgs() { View = view } );
        return view;
      }

    }

    /// <summary>
    /// 尝试创建自定义视图处理程序对象
    /// </summary>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <param name="isPartial">是否应创建为部分视图</param>
    /// <returns>若有自定义视图处理程序，则返回。</returns>
    protected virtual ViewBase TryCreateViewHandler( string virtualPath, bool isPartial )
    {
      var handlerPath = virtualPath + ".ashx";

      if ( !VirtualPathProvider.FileExists( handlerPath ) )
        return null;

      var view = (ViewBase) BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( ViewBase ) );
      if ( view == null )
        return null;

      return view;
    }



    static JumonyViewEngine()
    {
      ViewProviders = new SynchronizedCollection<IViewProvider>( _providersSync );
    }



    private static object _providersSync = new object();

    /// <summary>
    /// 获取或设置自定义视图提供程序
    /// </summary>
    public static ICollection<IViewProvider> ViewProviders
    {
      get;
      private set;
    }




    /// <summary>
    /// 当视图被成功创建时发生
    /// </summary>
    public event EventHandler<JumonyViewEventArgs> ViewCreated;

    /// <summary>
    /// 引发 ViewCreated 事件
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnViewCreated( JumonyViewEventArgs e )
    {
      if ( ViewCreated != null )
        ViewCreated( this, e );

      MvcEnvironment.OnViewCreated( this, e );
    }


  }

  /// <summary>
  /// 为 Jumony 视图引擎事件提供参数
  /// </summary>
  public class JumonyViewEventArgs : EventArgs
  {


    /// <summary>
    /// 获取创建的视图对象
    /// </summary>
    public ViewBase View
    {
      get;
      internal set;
    }


    /// <summary>
    /// 获取创建视图对象的 ViewProvider ，若不是由自定义视图提供程序创建，则为 null
    /// </summary>
    public IViewProvider ViewProvider
    {
      get;
      internal set;
    }

  }

}
