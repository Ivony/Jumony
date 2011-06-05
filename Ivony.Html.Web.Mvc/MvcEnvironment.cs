using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod( typeof( Ivony.Html.Web.Mvc.MvcEnvironment ), "Initialize" )]

namespace Ivony.Html.Web.Mvc
{

  public static class MvcEnvironment
  {

    public static void Initialize()
    {
      ViewEngines.Engines.Add( _viewEngine );
    }

    private static JumonyViewEngine _viewEngine = new JumonyViewEngine();

    public static JumonyViewEngine JumonyViewEngine
    {
      get { return _viewEngine; }
    }





    public static string GetCacheKey( ControllerContext context, ActionDescriptor actionDescriptor )
    {

      return HtmlProviders.GetCacheKey( context.HttpContext );

    }


    public static HtmlCachePolicy GetCachePolicy( ControllerContext context, ActionDescriptor actionDescriptor, ActionResult cacheItem )
    {

      return HtmlProviders.GetCachePolicy( context.HttpContext, null );

    }



  }
}
