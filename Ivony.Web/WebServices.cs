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
  /// Web 服务注册点
  /// </summary>
  public static class WebServices
  {

    private static object sync = new object();


    private static ArrayList globalServices = ArrayList.Synchronized( new ArrayList() );

    private static Hashtable serviceMap = Hashtable.Synchronized( new Hashtable() );

    private static Hashtable servicesCache = Hashtable.Synchronized( new Hashtable() );



    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <param name="service">要注册的服务对象</param>
    public static void RegisterService( object service )
    {
      lock ( sync )
      {
        globalServices.Insert( 0, service );
        return;
      }
    }


    /// <summary>
    /// 注册一个服务
    /// </summary>
    /// <param name="service">服务对象</param>
    /// <param name="virtualPath">所适用的虚拟路径范围</param>
    public static void RegisterService( object service, string virtualPath )
    {


      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );



      lock ( sync )
      {
        servicesCache.Clear();

        var serviceCollection = serviceMap[virtualPath] as ArrayList;
        if ( serviceCollection == null )
          serviceMap[virtualPath] = serviceCollection = new ArrayList();

        serviceCollection.Insert( 0, service );
      }
    }


    /// <summary>
    /// 取消注册服务
    /// </summary>
    /// <param name="service">要取消注册的服务对象</param>
    /// <param name="backtracking">是否要清理所有路径的注册，如果设置为 false 则只清理注册在全局服务</param>
    public static void UnregisterService( object service, bool backtracking = true )
    {
      if ( service == null )
        throw new ArgumentNullException( "service" );


      lock ( sync )
      {
        servicesCache.Clear();

        globalServices.Remove( service );

        if ( backtracking )
        {
          foreach ( string path in serviceMap.Keys )
          {
            UnregisterCore( path, service );
          }
        }

      }
    }


    /// <summary>
    /// 在指定虚拟路径上取消注册服务
    /// </summary>
    /// <param name="service">要取消注册的服务对象</param>
    /// <param name="virtualPath">要取消注册的虚拟路径</param>
    /// <param name="backtracking">是否要上溯清理所有父级路径的注册，如果设置为 false 则只清理当前路径的注册</param>
    public static void UnregisterService( object service, string virtualPath, bool backtracking = true )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( service == null )
        throw new ArgumentNullException( "service" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );


      lock ( sync )
      {
        servicesCache.Clear();

        UnregisterCore( virtualPath, service );

        if ( backtracking )
        {

          string parent;

          if ( virtualPath == "~/" )
            parent = null;

          else if ( virtualPath.EndsWith( "/" ) )
            parent = VirtualPathUtility.Combine( virtualPath, "../" );

          else
            parent = VirtualPathUtility.GetDirectory( virtualPath );

          if ( parent != null )
            UnregisterService( service, parent, true );
        }

      }

    }

    private static void UnregisterCore( string virtualPath, object service )
    {
      var services = serviceMap[virtualPath] as ArrayList;
      if ( services == null || services.Count == 0 )
        return;

      services.Remove( service );
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

      lock ( sync )
      {

        var services = servicesCache[virtualPath] as object[];
        if ( services == null )
        {
          string directory = VirtualPathUtility.GetDirectory( virtualPath );
          servicesCache[virtualPath] = services = GetServices( virtualPath ).Concat( GetServicesFromServiceMap( directory ) ).ToArray();
        }

        return services.OfType<T>().Concat( GetServices<T>() ).ToArray();
      }
    }


    /// <summary>
    /// 获取指定虚拟路径中注册的服务
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    private static object[] GetServices( string virtualPath )
    {

      var services = serviceMap[virtualPath] as ArrayList;
      if ( services != null )
        return services.Cast<object>().ToArray();

      else
        return new object[0];

    }


    /// <summary>
    /// 从注册服务列表中检索服务
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <returns>该虚拟路径注册的所有服务对象</returns>
    private static object[] GetServicesFromServiceMap( string virtualPath )
    {

      if ( virtualPath == null )
        return new object[0];

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );

      var services = servicesCache[virtualPath] as object[];
      if ( services == null )
      {

        string parent = null;
        if ( virtualPath != "~/" )
          parent = VirtualPathUtility.Combine( virtualPath, "../" );

        servicesCache[virtualPath] = services = GetServices( virtualPath ).Concat( GetServicesFromServiceMap( parent ) ).ToArray();
      }


      return services;
    }




    /// <summary>
    /// 获取注册的全局服务对象
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns>服务对象</returns>
    public static T[] GetServices<T>() where T : class
    {
      lock ( sync )
      {
        return globalServices.OfType<T>().ToArray();
      }
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
