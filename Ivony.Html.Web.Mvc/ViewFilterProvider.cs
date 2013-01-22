using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  public static class ViewFilterProvider
  {

    public static IViewFilter[] GetViewFilters( string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return new IViewFilter[0];

      var directory = VirtualPathUtility.GetDirectory( virtualPath );


      var filters = new List<IViewFilter>();

      while ( true )
      {

        var handlerPath = VirtualPathUtility.Combine( directory, "_filter.ashx" );

        if ( VirtualPathProvider.FileExists( handlerPath ) )
        {

          var filter = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( ViewFilterHandler ) ) as IViewFilter;
          if ( filter != null )
            filters.Add( filter );

        }


        if ( directory == "~/" )
          break;

        directory = VirtualPathUtility.Combine( directory, "../" );
      }


      return filters.ToArray();

    }


    public static VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

  }
}
