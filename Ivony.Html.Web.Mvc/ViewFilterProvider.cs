using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  
  /// <summary>
  /// 视图筛选器提供程序
  /// </summary>
  public static class ViewFilterProvider
  {

    /// <summary>
    /// 获取指定虚拟路径的视图筛选器
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>视图筛选器</returns>
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


    /// <summary>
    /// 获取当前的虚拟路径提供程序
    /// </summary>
    public static VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

  }
}
