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




namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// IMvcCachePolicyProvider 的一个标准抽象实现
  /// </summary>
  public abstract class ControllerCachePolicyProvider : IMvcCachePolicyProvider
  {

    private static readonly string typeNamePostfix = "CachePolicyProvider";


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
    private Dictionary<string,ActionCachePolicyProvider> actionProviders;


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



    public static CacheToken CreateToken( string typeName, IDictionary<string, object> parameters )
    {
      return CacheToken.CreateToken( typeName, parameters.Select( pair => pair.Key + ":" + pair.Value ).ToArray() );
    }




    private static ControllerCachePolicyProvider[] _providers;
    private static object _providersSync = new object();



    private static ControllerCachePolicyProvider[] Providers
    {
      get
      {
        lock ( _providersSync )
        {
          if ( _providers == null )
            _providers = InitializeProviders();

          return _providers;
        }
      }
    }



    private static ControllerCachePolicyProvider[] InitializeProviders()
    {
      var providerBaseType = typeof( ControllerCachePolicyProvider );

      var types = BuildManager.GetReferencedAssemblies()
        .Cast<Assembly>()
        .SelectMany( assembly => assembly.GetExportedTypes() )
        .Where( type => type.Name.EndsWith( typeNamePostfix ) )
        .Where( type => type.IsSubclassOf( providerBaseType ) )
        .ToArray();

      var list = new List<ControllerCachePolicyProvider>();

      types.ForAll( t =>
      {
        try
        {
          list.Add( (ControllerCachePolicyProvider) Activator.CreateInstance( t ) );
        }
        catch
        {

        }
      } );

      return list.ToArray();
    }


    /// <summary>
    /// 获取全局缓存筛选器，在 ASP.NET 3 中，将此筛选器注册到全局范畴，即可自动应用应用程序内所有的 ControllerCacheProvider
    /// </summary>
    public static ActionFilterAttribute GlobalCacheFilter
    {
      get;
      private set;
    }

    static ControllerCachePolicyProvider()
    {
      GlobalCacheFilter = new GlobalControllerCacheFilter();
    }



    private delegate CachePolicy ActionCachePolicyProvider( ControllerContext context, IDictionary<string, object> parameters );

    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true )]
    private class GlobalControllerCacheFilter : CacheFilterBase
    {

      /// <summary>
      /// 创建缓存策略
      /// </summary>
      /// <param name="context">当前 MVC 上下文</param>
      /// <param name="action">当前调用的 Action</param>
      /// <param name="parameters">Action 的参数</param>
      /// <returns></returns>
      protected override CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
      {
        foreach ( var provider in Providers )
        {

          var policy = provider.CreateCachePolicy( context, action, parameters );
          if ( policy != null )
            return policy;

        }

        return null;
      }
    }



  }
}
