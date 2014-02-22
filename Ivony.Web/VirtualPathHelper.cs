using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Web
{

  /// <summary>
  /// 为常见的虚拟路径操作提供额外的实用工具方法。
  /// </summary>
  public static class VirtualPathHelper
  {

    /// <summary>
    /// 在指定虚拟路径上溯搜索指定文件名的文件
    /// </summary>
    /// <param name="virtualPath">要搜索的虚拟路径</param>
    /// <param name="fileNames">要搜索的文件名列表</param>
    /// <returns>返回找到的文件路径，若无法找到匹配的文件，则返回null</returns>
    public static string FallbackSearch( string virtualPath, params string[] fileNames )
    {
      return FallbackSearch( HostingEnvironment.VirtualPathProvider, virtualPath, fileNames );
    }



    /// <summary>
    /// 在指定虚拟路径上溯搜索指定文件名的文件
    /// </summary>
    /// <param name="provider">自定义的虚拟路径提供程序</param>
    /// <param name="virtualPath">要搜索的虚拟路径</param>
    /// <param name="fileNames">要搜索的文件名列表</param>
    /// <returns>返回找到的文件路径，若无法找到匹配的文件，则返回null</returns>
    public static string FallbackSearch( VirtualPathProvider provider, string virtualPath, params string[] fileNames )
    {
      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );


      while ( true )
      {

        virtualPath = GetParentDirectory( virtualPath );
        if ( virtualPath == null )
          break;

        foreach ( var name in fileNames )
        {
          var filePath = VirtualPathUtility.Combine( virtualPath, name );
          if ( provider.FileExists( filePath ) )
            return filePath;
        }
      }

      return null;
    }


    /// <summary>
    /// 获取父级目录
    /// </summary>
    /// <param name="virtualPath">要获取父级目录的虚拟路径</param>
    /// <returns>父级目录，若当前路径无法上溯，则返回 null</returns>
    public static string GetParentDirectory( string virtualPath )
    {
      if ( virtualPath == "~/" )
        return null;

      return VirtualPathUtility.GetDirectory( VirtualPathUtility.RemoveTrailingSlash( virtualPath ) );

    }


    /// <summary>
    /// 此方法仅供系统调用
    /// </summary>
    /// <param name="paramName">参数名称</param>
    /// <returns></returns>
    [System.ComponentModel.EditorBrowsable( System.ComponentModel.EditorBrowsableState.Never )]
    public static Exception VirtualPathFormatError( string paramName )
    {
      return new ArgumentException( string.Format( CultureInfo.InvariantCulture, "{0} 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", paramName ), paramName );
    }

  }
}
