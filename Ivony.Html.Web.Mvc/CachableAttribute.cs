using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Hosting;
using Ivony.Fluent;


namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// 用于指定 Action 或 Controller 应缓存输出结果。
  /// </summary>
  [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true )]
  public class CacheableAttribute : CacheFilterBase
  {


    public CacheableAttribute()
    {
    }



    /// <summary>
    /// 利用指定的缓存策略提供程序创建 HtmlCacheableAttribute 对象。
    /// </summary>
    /// <param name="policyProviderType">缓存策略提供程序类型</param>
    public CacheableAttribute( Type policyProviderType )
    {
      if ( policyProviderType.IsSubclassOf( typeof( IMvcCachePolicyProvider ) ) )
      {
        CachePolicyProvider = Activator.CreateInstance( policyProviderType ).CastTo<IMvcCachePolicyProvider>();
        return;
      }

      else if ( policyProviderType.IsSubclassOf( typeof( ICachePolicyProvider ) ) )
      {
        CachePolicyProvider = new MvcCachePolicyProviderWrapper( Activator.CreateInstance( policyProviderType ).CastTo<ICachePolicyProvider>() );
        return;
      }

      throw new InvalidOperationException( "配置错误，类型必须从 IHtmlCachePolicyProvider 或 IMvcCachePolicyProvider 派生" );
    }




    /// <summary>
    /// 获取缓存策略
    /// </summary>
    /// <param name="context">MVC 请求上下文</param>
    /// <returns></returns>
    protected override CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {

      if ( CachePolicyProvider != null )
        return CachePolicyProvider.CreateCachePolicy( context, action, parameters );



      var httpMethod = context.HttpContext.Request.HttpMethod;
      if ( !httpMethod.EqualsIgnoreCase( "get" ) && !httpMethod.EqualsIgnoreCase( "header" ) )//如果不是GET或Header请求，都不予缓存。
        return null;


      var provider = context.Controller as IMvcCachePolicyProvider;
      if ( provider == null )
      {
        var _provider = context.Controller as ICachePolicyProvider;
        if ( _provider != null )
          provider = new MvcCachePolicyProviderWrapper( _provider );
      }

      var policy = provider.CreateCachePolicy( context, action, parameters );

      if ( policy != null )
        return policy;



      return MvcEnvironment.CreateCachePolicy( context, action, parameters );

    }




    /// <summary>
    /// 获取缓存策略提供程序
    /// </summary>
    protected IMvcCachePolicyProvider CachePolicyProvider
    {
      get;
      set;
    }








  }
}
