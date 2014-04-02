using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;

namespace Ivony.Web
{


  /// <summary>
  /// 提供关于 Web 开发的一些帮助方法
  /// </summary>
  public static class WebExtensions
  {

    /// <summary>
    /// 枚举指定虚拟目录下所有文件
    /// </summary>
    /// <param name="directory">虚拟目录</param>
    /// <returns>该目录下所有虚拟文件</returns>
    public static IEnumerable<VirtualFile> EnumerateFiles( this VirtualDirectory directory )
    {

      foreach ( VirtualFile file in directory.Files )
        yield return file;

      foreach ( VirtualDirectory subDirectory in directory.Directories )
      {
        foreach ( VirtualFile file in EnumerateFiles( subDirectory ) )
          yield return file;
      }


    }



    /// <summary>
    /// 获取当前请求相对于应用程序根的虚拟路径
    /// </summary>
    /// <param name="request">HTTP 内部请求对象</param>
    /// <returns>相对于应用程序根的虚拟路径</returns>
    public static string GetVirtualPath( this HttpRequestBase request )
    {
      return request.AppRelativeCurrentExecutionFilePath + request.PathInfo;
    }

  }
}
