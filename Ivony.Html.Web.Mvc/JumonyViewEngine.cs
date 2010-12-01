using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Compilation;

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
      return new JumonyView( partialPath, LoadDocument( partialPath ) );
    }


    protected override IView CreateView( ControllerContext controllerContext, string viewPath, string masterPath )
    {
      if ( !string.IsNullOrEmpty( masterPath ) )
        throw new NotSupportedException();

      return CreateView( viewPath, LoadDocument( viewPath ) );
    }

    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      var file = VirtualPathProvider.GetFile( virtualPath );
      var content = StaticFileLoader.LoadContent( file );
    }



    static JumonyViewEngine()
    {
      ViewProviders = new SynchronizedCollection<IViewProvider>( _providersSync );
    }

    public static JumonyView CreateView( string virtualPath, IHtmlDocument document )
    {
      lock ( _providersSync )
      {
        foreach ( var provider in ViewProviders )
        {
          var view = provider.TryCreateView( virtualPath, document );
          if ( view != null )
            return view;
        }

        return new JumonyView( virtualPath, document );

      }
    }


    private static object _providersSync = new object();

    public static ICollection<IViewProvider> ViewProviders
    {
      get;
      private set;
    }

  }
}
