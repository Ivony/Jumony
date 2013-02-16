using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Ivony.Web
{
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

  }
}
