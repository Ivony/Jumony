using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Web;
using System.Web.Mvc;
using Ivony.Fluent;
using System.Web.Compilation;
using System.Reflection;
using System.Web;
using Ivony.Web;




namespace Ivony.Html.Web
{
  /// <summary>
  /// 基于 Controller 和 Action 名称提供缓存策略的缓存策略提供程序
  /// </summary>
  public abstract class ControllerCachePolicyProvider : IMvcCachePolicyProvider
  {

    /// <summary>
    /// 创建 ControllerCachePolicyProvider 实例
    /// </summary>
    protected ControllerCachePolicyProvider()
    {
      actionProviders = type.GetMethods()
        .Where( method => method.ReturnType == typeof( CachePolicy ) )
        .Where( method => method.GetParameters().Select( p => p.ParameterType ).SequenceEqual( new[] { typeof( ControllerContext ), typeof( IDictionary<string, object> ) } ) )
        .ToDictionary( method => method.Name, method => (ActionCachePolicyProvider) Delegate.CreateDelegate( typeof( ActionCachePolicyProvider ), this, method ) );


    }

    private Type type;
    private Dictionary<string, ActionCachePolicyProvider> actionProviders;


    public abstract string ControllerName
    {
      get;
    }



    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context">当前请求的 MVC 上下文</param>
    /// <param name="action">当前执行的 Action</param>
    /// <param name="parameters">Action 的参数</param>
    /// <returns></returns>
    public CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {

      if ( MatchController( context, action.ControllerDescriptor.ControllerName ) )
        return CreateActionCachePolicy( context, action, parameters );

      else
        return null;

    }

    /// <summary>
    /// 创建 Action 的缓存策略
    /// </summary>
    /// <param name="context">当前控制器上下文</param>
    /// <param name="action">当前执行的 Action</param>
    /// <param name="parameters">Action 的参数</param>
    /// <returns>缓存策略</returns>
    protected virtual CachePolicy CreateActionCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      ActionCachePolicyProvider actionProvider;

      if ( !actionProviders.TryGetValue( action.ActionName, out actionProvider ) )
        return null;


      return actionProvider( context, parameters );
    }



    protected bool MatchController( ControllerContext context, string controllerName )
    {
      if ( ControllerName.Contains( "." ) )
        return context.Controller.GetType().FullName.EqualsIgnoreCase( ControllerName );

      else if ( ControllerName.Contains( ":" ) )
      {
        var frags = ControllerName.Split( ':' );
        var area = frags[0];
        var controller = frags[1];

        if ( !context.RouteData.Values.ContainsKey( "area" ) )
          return false;

        var _area = context.RouteData.Values["area"] as string;
        if ( !area.EqualsIgnoreCase( _area ) )
          return false;

        return frags[1].EqualsIgnoreCase( controllerName );
      }

      else
        return ControllerName.EqualsIgnoreCase( controllerName );
    }



    CachePolicy ICachePolicyProvider.CreateCachePolicy( HttpContextBase context )
    {
      return null;
    }


    /// <summary>
    /// 创建 CacheToken
    /// </summary>
    /// <param name="typeName">类型名，一般可以取 Action 的名称</param>
    /// <param name="parameters">参数列表，一般可以使用 Action 的参数列表</param>
    /// <returns>针对指定类型和参数的 CacheToken</returns>
    public static CacheToken CreateToken( string typeName, IDictionary<string, object> parameters )
    {
      return CacheToken.CreateToken( typeName, parameters.Select( pair => pair.Key + ":" + pair.Value ).ToArray() );
    }




    private static Dictionary<string, ControllerCachePolicyProvider> _providers = new Dictionary<string, ControllerCachePolicyProvider>();
    private static object _providersSync = new object();



    /// <summary>
    /// 注册控制器缓存策略提供程序
    /// </summary>
    /// <param name="provider">缓存策略提供程序</param>
    public static void RegisterCacheProvider( ControllerCachePolicyProvider provider )
    {
      lock ( _providersSync )
      {
        var name = provider.ControllerName;
        if ( _providers.ContainsKey( name ) )
          throw new InvalidOperationException( string.Format( "已经注册了名为 \"{0}\" 的控制器缓存策略提供程序", name ) );

        _providers.Add( name, provider );
      }
    }



    private static Dictionary<string, ControllerCachePolicyProvider> InitializeProviders()
    {
      var providerBaseType = typeof( ControllerCachePolicyProvider );

      var types = BuildManager.GetReferencedAssemblies()
        .Cast<Assembly>()
        .SelectMany( assembly => assembly.GetExportedTypes() )
        .Where( type => type.Name.EndsWith( typeNamePostfix ) )
        .Where( type => type.IsSubclassOf( providerBaseType ) )
        .ToArray();

      var result = new Dictionary<string, ControllerCachePolicyProvider>();

      types.ForAll( t =>
      {
        try
        {
          var instance = (ControllerCachePolicyProvider) Activator.CreateInstance( t );
          result.Add( instance.ControllerName, instance );
        }
        catch
        {

        }
      } );

      return result;
    }


    /// <summary>
    /// 获取缓存策略提供程序
    /// </summary>
    /// <param name="controllerName">控制器名称</param>
    /// <returns>该控制器的缓存策略提供程序，如果有的话</returns>
    internal static ControllerCachePolicyProvider GetProvider( string controllerName )
    {
      lock ( _providersSync )
      {
        ControllerCachePolicyProvider provider;

        if ( _providers.TryGetValue( controllerName, out provider ) )
          return provider;
      }

      return null;
    }





    private delegate CachePolicy ActionCachePolicyProvider( ControllerContext context, IDictionary<string, object> parameters );





  }
}
