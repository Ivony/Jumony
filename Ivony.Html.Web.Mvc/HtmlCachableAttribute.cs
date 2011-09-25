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
      if ( policyProviderType.IsSubclassOf( typeof( IMvcCachePolicyProvider ) ) )
      {
        CachePolicyProvider = Activator.CreateInstance( policyProviderType ).CastTo<IMvcCachePolicyProvider>();
        return;
      }

      else if ( policyProviderType.IsSubclassOf( typeof( IHtmlCachePolicyProvider ) ) )
      {
        CachePolicyProvider = new MvcCachePolicyProvider( Activator.CreateInstance( policyProviderType ).CastTo<IHtmlCachePolicyProvider>() );
        return;
      }

      throw new InvalidOperationException( "配置错误，类型必须从 IHtmlCachePolicyProvider 或 IMvcCachePolicyProvider 派生" );
    }





    private static readonly string CachePolicyToken = "JumonyforMVC_CacheControl_CachePolicy";
    private static readonly string CacheHitToken = "JumonyforMVC_CacheControl_CacheHit";
    private static readonly string ClientCacheResolved = "JumonyforMVC_CacheControl_ClientCacheResolved";


    /// <summary>
    /// 重写此方法以拦截请求并输出缓存
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {

      if ( ResolveCache( filterContext ) )
      {
        filterContext.HttpContext.Trace.Write( "Jumony for MVC", "OutputCache hitted" );
        filterContext.RouteData.DataTokens[CacheHitToken] = "Hitted";
      }
      else
        filterContext.HttpContext.Trace.Write( "Jumony for MVC", "OutputCache missed" );

    }


    /// <summary>
    /// 尝试输出缓存
    /// </summary>
    /// <param name="filterContext"></param>
    /// <returns></returns>
    protected virtual bool ResolveCache( ActionExecutingContext filterContext )
    {

      var cachePolicy = GetCachePolicy( filterContext, filterContext.ActionDescriptor );

      if ( cachePolicy == null )
        return false;



      filterContext.RouteData.DataTokens[CachePolicyToken] = cachePolicy;


      var requestCacheControl = filterContext.HttpContext.Request.Headers["cache-control"];
      if ( requestCacheControl.EqualsIgnoreCase( "no-cache" ) )
        return false;




      var cachable = cachePolicy as IClientCachablePolicy;
      if ( cachable != null )
      {
        if ( cachable.ResolveClientCache( filterContext.HttpContext ) )
        {
          filterContext.Result = new EmptyResult();
          filterContext.RouteData.DataTokens[ClientCacheResolved] = "Resolved";
          return true;
        }
      }


      var response = filterContext.HttpContext.Cache.GetCachedResponse( cachePolicy.CacheToken );

      if ( response != null )
      {
        filterContext.Result = response.ToCachedResult();
        return true;
      }


      return false;
    }



    /// <summary>
    /// 获取缓存策略
    /// </summary>
    /// <param name="context">MVC 请求上下文</param>
    /// <returns></returns>
    protected virtual CachePolicy GetCachePolicy( ControllerContext context, ActionDescriptor action )
    {

      if ( CachePolicyProvider != null )
      {
        var policy = CachePolicyProvider.GetCachePolicy( context, action );

        if ( policy != null )
          return policy;
      }

      else
      {

        var provider = context.Controller as IMvcCachePolicyProvider;
        if ( provider == null )
        {
          var _provider = context.Controller as IHtmlCachePolicyProvider;
          if ( _provider != null )
            provider = new MvcCachePolicyProvider( _provider );
        }

        var policy = provider.GetCachePolicy( context, action );

        if ( policy != null )
          return policy;
      }


      return MvcEnvironment.GetCachePolicy( context, action );

    }










    /// <summary>
    /// 获取缓存策略提供程序
    /// </summary>
    protected IMvcCachePolicyProvider CachePolicyProvider
    {
      get;
      set;
    }



    public override void OnResultExecuting( ResultExecutingContext filterContext )
    {

      ApplyClientCachePolicy( filterContext );

    }

    protected void ApplyClientCachePolicy( ResultExecutingContext filterContext )
    {
      //对于子请求不采取客户端缓存。
      if ( filterContext.IsChildAction )
        return;

      var dataTokens = filterContext.RouteData.DataTokens;

      //若已命中缓存，则不再应用客户端缓存策略
      if ( dataTokens[ClientCacheResolved] != null )
        return;


      var cachePolicy = dataTokens[CachePolicyToken] as CachePolicy;

      cachePolicy.ApplyClientCachePolicy( filterContext.HttpContext.Response.Cache );
    }




    /// <summary>
    /// 重写此方法使得操作结果执行完毕后，更新缓存信息
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnResultExecuted( ResultExecutedContext filterContext )
    {
      UpdateCache( filterContext );
    }


    private void UpdateCache( ResultExecutedContext filterContext )
    {
      //若已命中缓存，则不再更新缓存
      if ( filterContext.RouteData.DataTokens[CacheHitToken] != null )
        return;

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
    /// <param name="context">当前 Controller 上下文</param>
    protected void UpdateCache( ICachedResponse cached, ControllerContext context )
    {

      //对于子请求不予缓存。
      if ( context.IsChildAction )
        return;


      var httpContext = context.HttpContext;

      var cachePolicy = context.RouteData.DataTokens[CachePolicyToken] as CachePolicy;
      if ( cachePolicy == null )
        return;

      UpdateCache( cached, context, cachePolicy );
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
    /// <param name="cachedResponse">可被缓存的响应数据</param>
    /// <param name="context">MVC 请求上下文</param>
    /// <param name="policy">缓存策略</param>
    protected void UpdateCache( ICachedResponse cachedResponse, ControllerContext context, CachePolicy policy )
    {
      var cacheItem = policy.CreateCacheItem( cachedResponse );
      context.HttpContext.Cache.InsertCacheItem( cacheItem );
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
