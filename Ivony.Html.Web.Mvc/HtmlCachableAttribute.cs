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
  public class HtmlCachableAttribute : ActionFilterAttribute
  {


    public HtmlCachableAttribute()
    {
    }



    /// <summary>
    /// 利用指定的缓存策略提供程序创建 HtmlCachableAttribute 对象。
    /// </summary>
    /// <param name="policyProviderType">缓存策略提供程序类型</param>
    public HtmlCachableAttribute( Type policyProviderType )
    {
      if ( !policyProviderType.IsSubclassOf( typeof( IHtmlCachePolicyProvider ) ) )
        throw new InvalidOperationException( "配置错误，类型必须从 IHtmlCachePolicyProvider 派生" );


      CachePolicyProvider = Activator.CreateInstance( policyProviderType ).CastTo<IHtmlCachePolicyProvider>();

    }





    private static readonly string CacheKeyToken = "JumonyforMVC_CacheControl_CacheKey";


    /// <summary>
    /// 重写此方法以拦截请求并输出缓存
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {
      var cacheKey = GetCacheKey( filterContext, filterContext.ActionDescriptor );

      if ( cacheKey == null )
        return;

      filterContext.RouteData.DataTokens["JumonyforMVC_CacheControl_CacheKey"] = cacheKey;

      var response = HostingEnvironment.Cache[cacheKey] as ICachedResponse;

      if ( response != null )
      {
        filterContext.Result = response.ToCachedResult();
        filterContext.HttpContext.Trace.Write( "Jumony for MVC", "OutputCache hited" );
      }

      else
        filterContext.HttpContext.Trace.Write( "Jumony for MVC", "OutputCache missed" );

    }


    /// <summary>
    /// 获取缓存键
    /// </summary>
    /// <param name="context">MVC 请求上下文</param>
    /// <returns></returns>
    protected virtual string GetCacheKey( ControllerContext context, ActionDescriptor action )
    {
      if ( CachePolicyProvider != null )
        return CachePolicyProvider.GetCacheKey( context.HttpContext );


      var provider = context.Controller as IHtmlCachePolicyProvider;
      if ( provider != null )
        return provider.GetCacheKey( context.HttpContext );


      return MvcEnvironment.GetCacheKey( context, action );
    }






    /// <summary>
    /// 获取缓存策略提供程序
    /// </summary>
    protected IHtmlCachePolicyProvider CachePolicyProvider
    {
      get;
      set;
    }





    /// <summary>
    /// 重写此方法使得操作结果执行完毕后，更新缓存信息
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnResultExecuted( ResultExecutedContext filterContext )
    {
      var result = filterContext.Result;

      var cachedResponse = GetCachedResponse( result );

      if ( cachedResponse != null )
        UpdateCache( cachedResponse, filterContext );

      base.OnResultExecuted( filterContext );
    }



    /// <summary>
    /// 尝试更新缓存
    /// </summary>
    /// <param name="cached">用于缓存的响应信息</param>
    /// <param name="httpContext">当前 Controller 上下文</param>
    protected void UpdateCache( ICachedResponse cached, ControllerContext context )
    {

      //对于子请求不予缓存。
      if ( context.IsChildAction )
        return;


      string cacheKey;
      HtmlCachePolicy cachePolicy;

      var httpContext = context.HttpContext;

      cacheKey = context.RouteData.DataTokens[CacheKeyToken] as string;
      if ( cacheKey == null )
        return;

      cachePolicy = GetCachePolicy( context, cacheKey, cached );


      UpdateCache( cached, cacheKey, cachePolicy );
    }




    /// <summary>
    /// 获取缓存策略
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <param name="cached">已缓存的响应</param>
    /// <returns>缓存策略</returns>
    protected virtual HtmlCachePolicy GetCachePolicy( ControllerContext context, string cacheKey, ICachedResponse cached )
    {
      if ( CachePolicyProvider != null )
        return CachePolicyProvider.GetCachePolicy( context.HttpContext, cached );

      var provider = context.Controller as IHtmlCachePolicyProvider;
      if ( provider != null )
        return provider.GetCachePolicy( context.HttpContext, cached );


      else
        return MvcEnvironment.GetCachePolicy( context, cacheKey, cached );
    }



    /// <summary>
    /// 从执行结果中获取可被缓存的响应数据
    /// </summary>
    /// <param name="result">Action 执行结果</param>
    /// <returns>可被缓存的响应数据，失败则返回 null</returns>
    protected ICachedResponse GetCachedResponse( ActionResult result )
    {

      var cachable = result as ICachableResult;
      if ( cachable != null )
        return cachable.GetCachedResponse();

      var viewResult = result as ViewResultBase;
      if ( viewResult != null )
      {
        cachable = viewResult.View as ICachableResult;
        if ( cachable != null )
          return cachable.GetCachedResponse();
      }

      var content = result as ContentResult;
      if ( content != null )
        return CreateCachedResponse( content );

      return null;

    }


    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="cacheItem">可被缓存的响应数据</param>
    /// <param name="cacheKey">缓存键</param>
    /// <param name="cachePolicy">缓存策略</param>
    protected void UpdateCache( ICachedResponse cacheItem, string cacheKey, HtmlCachePolicy cachePolicy )
    {
      HostingEnvironment.Cache.WriteCache( cacheItem, cacheKey, cachePolicy );
    }




    private ICachedResponse CreateCachedResponse( ContentResult content )
    {
      var response = new RawResponse()
      {
        Content = content.Content,
        ContentEncoding = content.ContentEncoding,
      };

      response.Headers.Add( "ContentType", content.ContentType );

      return response;
    }
  }
}
