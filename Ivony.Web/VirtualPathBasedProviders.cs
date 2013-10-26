using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using Ivony.Fluent;
using System.Globalization;
using System.Web.Hosting;

namespace Ivony.Web
{
  /// <summary>
  /// 基于虚拟路径的服务提供程序
  /// </summary>
  public static class VirtualPathBasedProvider
  {

    private static object sync = new object();


    private static ArrayList globalServices = ArrayList.Synchronized( new ArrayList() );

    private static Hashtable serviceMap = Hashtable.Synchronized( new Hashtable() );

    private static Hashtable servicesCache = Hashtable.Synchronized( new Hashtable() );


    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <param name="service">服务对象</param>
    /// <param name="virtualPath">所适用的虚拟路径范围</param>
    public static void RegisterService( object service, string virtualPath = null )
    {


      if ( virtualPath == null )
      {
        lock ( sync )
        {
          globalServices.Add( service );
          return;
        }
      }



      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );



      lock ( sync )
      {
        servicesCache = null;

        var serviceCollection = serviceMap[virtualPath] as ArrayList;
        if ( serviceCollection == null )
          serviceMap[virtualPath] = serviceCollection = new ArrayList();

        serviceCollection.Add( service );
      }
    }



    /// <summary>
    /// 获取指定虚拟路径所有服务对象
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>该虚拟路径注册的所有服务对象</returns>
    public static T[] GetServices<T>( string virtualPath ) where T : class
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );

      string directory = VirtualPathUtility.GetDirectory( virtualPath );

      lock ( sync )
      {

        var services = servicesCache[directory] as object[];
        if ( services == null )
          servicesCache[directory] = services = GetServicesFromServiceMap( directory );

        return services.OfType<T>().ToArray();

      }
    }



    /// <summary>
    /// 从注册服务列表中检索服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>该虚拟路径注册的所有服务对象</returns>
    private static object[] GetServicesFromServiceMap( string virtualPath )
    {

      if ( virtualPath == null )
        return new object[0];

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );


      string parent = null;
      if ( virtualPath != "~/" )
        parent = VirtualPathUtility.Combine( virtualPath, "../" );


      var services = (serviceMap[virtualPath] as ArrayList).Cast<object>();

      if ( services != null )
        return services.Concat( GetServicesFromServiceMap( parent ) ).ToArray();

      else
        return services.ToArray();
    }


    /// <summary>
    /// 此方法仅供框架调用
    /// </summary>
    /// <param name="paramName">参数名称</param>
    /// <returns></returns>
    public static Exception VirtualPathFormatError( string paramName )
    {
      return new ArgumentException( string.Format( CultureInfo.InvariantCulture, "{0} 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", paramName ), paramName );
    }

  }
}
