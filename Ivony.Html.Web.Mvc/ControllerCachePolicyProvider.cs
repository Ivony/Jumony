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
  /// IMvcCachePolicyProvider 的一个标准抽象实现
  /// </summary>
  public abstract class ControllerCachePolicyProvider : IMvcCachePolicyProvider
  {

    private static readonly string typeNamePostfix = "CachePolicyProvider";

    /// <summary>
    /// 创建 ControllerCachePolicyProvider 实例
    /// </summary>
    protected ControllerCachePolicyProvider()
    {
      type = GetType();
      if ( !type.Name.EndsWith( typeNamePostfix ) )
        throw new InvalidOperationException( "派生类类名不正确，应以 " + typeNamePostfix + " 结尾" );

      controllerName = type.Name.Substring( 0, type.Name.Length - typeNamePostfix.Length );


      actionProviders = type.GetMethods()
        .Where( method => method.ReturnType == typeof( CachePolicy ) )
        .Where( method => method.GetParameters().Select( p => p.ParameterType ).SequenceEqual( new[] { typeof( ControllerContext ), typeof( IDictionary<string, object> ) } ) )
        .ToDictionary( method => method.Name, method => (ActionCachePolicyProvider) Delegate.CreateDelegate( typeof( ActionCachePolicyProvider ), this, method ) );


    }

    private Type type;
    private string controllerName;
    private Dictionary<string, ActionCachePolicyProvider> actionProviders;


    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context">当前请求的 MVC 上下文</param>
    /// <param name="action">当前执行的 Action</param>
    /// <param name="parameters">Action 的参数</param>
    /// <returns></returns>
    public CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      if ( !action.ControllerDescriptor.ControllerName.EqualsIgnoreCase( controllerName ) )
        return null;

      return CreateActionCachePolicy( context, action, parameters );

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
        var name = provider.controllerName;
        if ( _providers.ContainsKey( name ) )
          throw new InvalidOperationException( string.Format( "已经为名为 \"{0}\" 的控制器注册了缓存策略提供程序", name ) );

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
          result.Add( instance.controllerName, instance );
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
