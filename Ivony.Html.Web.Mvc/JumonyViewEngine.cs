using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Compilation;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 基于 Jumony 技术的视图引擎
  /// </summary>
  public class JumonyViewEngine : VirtualPathProviderViewEngine
  {

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
    /// 创建页视图
    /// </summary>
    /// <param name="controllerContext"></param>
    /// <param name="viewPath"></param>
    /// <param name="masterPath"></param>
    /// <exception cref="System.NotSupportedException">当 masterPath 参数不为空时引发，因为 JumonyViewEngine 不支持母板页</exception>
    /// <returns></returns>
    protected override IView CreateView( ControllerContext controllerContext, string viewPath, string masterPath )
    {
      if ( !string.IsNullOrEmpty( masterPath ) )
        throw new NotSupportedException();

      return CreateViewCore( controllerContext, viewPath, false );
    }


    /// <summary>
    /// 创建视图对象
    /// </summary>
    /// <param name="context"></param>
    /// <param name="virtualPath"></param>
    /// <param name="isPartial"></param>
    /// <returns></returns>
    protected IView CreateViewCore( ControllerContext context, string virtualPath, bool isPartial )
    {

      lock ( _providersSync )
      {
        foreach ( var provider in ViewProviders )
        {
          var view = provider.TryCreateView( context, VirtualPathProvider, virtualPath, isPartial );
          if ( view != null )
          {
            OnViewCreated( new JumonyViewEventArgs() { View = view, ViewProvider = provider } );
            return view;
          }
        }
      }


      {//默认处理策略
        var handlerPath = virtualPath + ".ashx";

        ViewBase view = null;

        if ( VirtualPathProvider.FileExists( handlerPath ) )
        {

          if ( isPartial )
          {
            view = CreateHandledPartialView( virtualPath, handlerPath );
          }
          else
          {
            view = CreateHandledPageView( virtualPath, handlerPath );
          }
        }


        if ( view == null )
        {
          if ( isPartial )
            view = new GenericPartialView( virtualPath );
          else
            view = new GenericPageView( virtualPath );
        }


        OnViewCreated( new JumonyViewEventArgs() { View = view } );
        return view;
      }

    }



    /// <summary>
    /// 创建带有指定处理器的页面视图
    /// </summary>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <param name="handlerPath">处理器虚拟路径</param>
    /// <returns></returns>
    protected static ViewBase CreateHandledPageView( string virtualPath, string handlerPath )
    {
      try
      {
        var handler = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( ViewHandler ) ) as ViewHandler;
        if ( handler != null )
        {

          handler.Initialize( virtualPath );

          return handler;

        }
      }
      catch
      {

      }

      return null;

    }

    /// <summary>
    /// 创建带有指定处理器的部分视图
    /// </summary>
    /// <param name="virtualPath">视图虚拟路径</param>
    /// <param name="handlerPath">处理器虚拟路径</param>
    /// <returns></returns>
    protected static ViewBase CreateHandledPartialView( string virtualPath, string handlerPath )
    {
      try
      {
        var handler = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( PartialViewHandler ) ) as PartialViewHandler;
        if ( handler != null )
        {

          handler.Initialize( virtualPath );

          return handler;

        }
      }
      catch
      {

      }

      return null;

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
