using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Web
{
  /// <summary>
  /// 基于虚拟路径的提供程序列表
  /// </summary>
  public static class VirtualPathBasedProviders
  {

    private static List<IVirtualPathBasedProvider> _providers = new List<IVirtualPathBasedProvider>();

    private static object _sync = new object();




    public static void RegisterProvider( IVirtualPathBasedProvider provider )
    {
      lock ( _sync )
      {
        _providers.Add( provider );
      }
    }


    public static T GetService<T>( string virtualPath ) where T : class
    {
      lock ( _sync )
      {
        foreach ( var p in _providers )
        {
          var service = p.GetService( virtualPath, typeof( T ) ) as T;

          if ( service != null )
            return service;
        }

        return null;
      }
    }


    public static T GetService<T>( string virtualPath, params IVirtualPathBasedProvider[] defaultProviders ) where T : class
    {
      var service = GetService<T>( virtualPath );

      if ( service != null )
        return service;


      foreach ( var p in defaultProviders )
      {
        service = p.GetService<T>( virtualPath );
        if ( service != null )
          return service;
      }

      return null;
    }




    /// <summary>
    /// 获取指定类型的服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="provider">服务提供程序</param>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>服务对象</returns>
    public static T GetService<T>( this IVirtualPathBasedProvider provider, string virtualPath ) where T : class
    {
      return provider.GetService( virtualPath, typeof( T ) ) as T;
    }




  }
}
