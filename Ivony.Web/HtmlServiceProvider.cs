using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  public class HtmlServiceProvider
  {

    private ArrayList services = ArrayList.Synchronized( new ArrayList() );
    private ArrayList providers = ArrayList.Synchronized( new ArrayList() );


    /// <summary>
    /// 获取当前系统中所有注册的指定类型的服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>已注册的服务列表</returns>
    public T[] GetServices<T>() where T : class
    {
      List<T> result = new List<T>();

      lock ( services.SyncRoot )
      {
        foreach ( var item in services )
        {
          var service = item as T;
          if ( service != null )
            result.Add( service );
        }
      }

      lock ( providers.SyncRoot )
      {
        foreach ( IServiceProvider provider in providers )
        {
          var service = provider.GetService( typeof( T ) ) as T;
          if ( service != null )
            result.Add( service );
        }
      }

      return result.ToArray();
    }


    /// <summary>
    /// 直接注册一个服务到系统中
    /// </summary>
    /// <param name="service">注册的服务对象</param>
    public void RegisterService( object service )
    {
      services.Add( service );
    }


    /// <summary>
    /// 注册一个服务提供程序到系统中
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    public void RegisterServiceProvider( IServiceProvider serviceProvider )
    {
      providers.Add( serviceProvider );
    }

  }
}
