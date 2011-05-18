using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Compilation;
using System.Web;

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
    /// <returns></returns>
    protected override IView CreateView( ControllerContext controllerContext, string viewPath, string masterPath )
    {
      if ( !string.IsNullOrEmpty( masterPath ) )
        throw new NotSupportedException();

      return CreateViewCore( controllerContext, viewPath, false );
    }


    /// <summary>
    /// 加载 HTML 文件
    /// </summary>
    /// <param name="context"></param>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    protected virtual IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {
      var content = new StaticFileLoader().LoadContent( context, VirtualPathProvider, virtualPath );//UNDONE 不应每次创建一个实例

      return HtmlProviders.ParseDocument( context, content );
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
          var view = provider.TryCreateView( context, virtualPath, isPartial );
          if ( view != null )
            return view;
        }
      }

      var handlerPath = virtualPath + ".ashx";
      if ( VirtualPathProvider.FileExists( handlerPath ) )
      {

        if ( isPartial )
        {

          var view = CreateHandledPartialView( virtualPath, handlerPath );
          if ( view != null )
            return view;

        }
        else
        {

          var view = CreateHandledPageView( virtualPath, handlerPath );
          if ( view != null )
            return view;

        }
      }



      if ( isPartial )
        return new GenericPartialView( virtualPath );

      else
        return new GenericPageView( virtualPath );

    }


    private static IView CreateHandledPageView( string virtualPath, string handlerPath )
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


    private static IView CreateHandledPartialView( string virtualPath, string handlerPath )
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

  }
}
