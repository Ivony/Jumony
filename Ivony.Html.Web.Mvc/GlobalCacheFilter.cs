using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;
using Ivony.Web;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 全局缓存筛选器，在 ASP.NET MVC 3 中将此筛选器加入全局筛选器，即可使得默认缓存策略运用于全局。
  /// </summary>
  public sealed class GlobalCacheFilter : CacheFilterBase
  {

    internal GlobalCacheFilter() { }

    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="action">Action 信息</param>
    /// <param name="parameters">Action 参数</param>
    /// <returns>缓存策略</returns>
    protected override CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {

      var httpMethod = context.HttpContext.Request.HttpMethod;
      if ( !httpMethod.EqualsIgnoreCase( "get" ) && !httpMethod.EqualsIgnoreCase( "header" ) )//如果不是GET或Header请求，都不予缓存。
        return null;


      //如果存在 Cacheable 筛选器，则不再提供默认策略。
      if ( action.GetCustomAttributes( typeof( CacheableAttribute ), true ).Any() || action.ControllerDescriptor.GetCustomAttributes( typeof( CacheableAttribute ), true ).Any() )
        return null;

      ControllerCachePolicyProvider provider = GetControllerProvider( MvcEnvironment.GetAreaName( context ), action.ControllerDescriptor.ControllerName );
      if ( provider != null )
      {
        var policy = provider.CreateCachePolicy( context, action, parameters );
        if ( policy != null )
          return policy;
      }


      return MvcEnvironment.CreateCachePolicy( context, action, parameters );
    }

    private ControllerCachePolicyProvider GetControllerProvider( string areaName, string controllerName )
    {

      ControllerCachePolicyProvider provider;

      string key;

      if ( string.IsNullOrEmpty( areaName ) )
        key = controllerName;
      else
        key = areaName + "." + controllerName;

      if ( _providers.TryGetValue( key, out provider ) )
        return provider;
      else
        return null;
    }


    private Dictionary<string, ControllerCachePolicyProvider> _providers = new Dictionary<string, ControllerCachePolicyProvider>();
    private object _providersSync = new object();



    /// <summary>
    /// 注册控制器缓存策略提供程序
    /// </summary>
    /// <param name="provider">缓存策略提供程序</param>
    public void RegisterCacheProvider( ControllerCachePolicyProvider provider, string controllerName, string areaName = null )
    {
      lock ( _providersSync )
      {
        var name = areaName + "." + controllerName;
        if ( _providers.ContainsKey( name ) )
        {
          if ( string.IsNullOrEmpty( areaName ) )
            throw new InvalidOperationException( string.Format( "已经为控制器 \"{0}\" 注册了缓存策略提供程序", controllerName ) );

          else
            throw new InvalidOperationException( string.Format( "已经为区域 \"{1}\" 的控制器 \"{0}\" 注册了缓存策略提供程序", controllerName, areaName ) );
        }

        _providers.Add( name, provider );
      }
    }


  }
}
