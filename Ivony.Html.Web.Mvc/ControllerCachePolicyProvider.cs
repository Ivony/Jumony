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
      InitializeActionProviders();
    }


    /// <summary>
    /// 初始化所有 Action 缓存提供程序
    /// </summary>
    protected void InitializeActionProviders()
    {
      actionProviders = GetType().GetMethods()
        .Where( method => method.ReturnType == typeof( CachePolicy ) )
        .Where( method => method.GetParameters().Select( p => p.ParameterType ).SequenceEqual( new[] { typeof( ControllerContext ), typeof( IDictionary<string, object> ) } ) )
        .ToDictionary( method => method.Name, method => (ActionCachePolicyProvider) Delegate.CreateDelegate( typeof( ActionCachePolicyProvider ), this, method ) );
    }

    private Dictionary<string, ActionCachePolicyProvider> actionProviders;



    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context">当前请求的 MVC 上下文</param>
    /// <param name="action">当前执行的 Action</param>
    /// <param name="parameters">Action 的参数</param>
    /// <returns>缓存策略</returns>
    public virtual CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
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


    private delegate CachePolicy ActionCachePolicyProvider( ControllerContext context, IDictionary<string, object> parameters );

  }
}
