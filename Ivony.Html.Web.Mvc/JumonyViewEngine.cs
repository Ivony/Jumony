using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Compilation;
using System.Web;

namespace Ivony.Html.Web.Mvc
{
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



    private static readonly JumonyViewEngine _instance = new JumonyViewEngine();

    public static JumonyViewEngine Instance
    {
      get { return _instance; }
    }


    private static readonly SynchronizedCollection<IViewProvider> _providers = new SynchronizedCollection<IViewProvider>();

    public static SynchronizedCollection<IViewProvider> Providers
    {
      get { return _providers; }
    }



    protected override IView CreatePartialView( ControllerContext controllerContext, string partialPath )
    {
      return CreateViewCore( controllerContext, partialPath, true );
    }


    protected override IView CreateView( ControllerContext controllerContext, string viewPath, string masterPath )
    {
      if ( !string.IsNullOrEmpty( masterPath ) )
        throw new NotSupportedException();

      return CreateViewCore( controllerContext, viewPath, false );
    }

    protected virtual IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {
      var content = new StaticFileLoader().LoadContent( context, VirtualPathProvider, virtualPath );//UNDONE 不应每次创建一个实例

      return HtmlProviders.ParseDocument( context, content );
    }



    static JumonyViewEngine()
    {
      ViewProviders = new SynchronizedCollection<IViewProvider>( _providersSync );
    }

    public IView CreateViewCore( ControllerContext context, string virtualPath, bool isPartial )
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
          

          
          throw new NotSupportedException();//UNDONE 
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


    private static object _providersSync = new object();

    public static ICollection<IViewProvider> ViewProviders
    {
      get;
      private set;
    }

  }
}
