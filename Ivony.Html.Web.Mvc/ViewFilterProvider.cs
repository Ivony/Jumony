using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供视图筛选器
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
        throw WebServices.VirtualPathFormatError( "virtualPath" );


      var filterProviders = WebServices.GetServices<IViewFilterProvider>( virtualPath );


      return filterProviders.SelectMany( p => p.GetFilters( virtualPath ) ).ToArray();

    }


  }
}
