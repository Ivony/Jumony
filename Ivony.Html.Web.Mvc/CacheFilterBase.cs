using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{


  /// <summary>
  /// 输出缓存筛选器
  /// </summary>
  public abstract class CacheFilterBase : ActionFilterAttribute
  {

    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {
      ResolveCache( filterContext );
    }


    private static readonly string CachePolicyToken = "JumonyforMVC_CacheControl_CachePolicy";
    private static readonly string CacheHitToken = "JumonyforMVC_CacheControl_CacheHit";
    private static readonly string ClientCacheResolved = "JumonyforMVC_CacheControl_ClientCacheResolved";


    protected virtual void ResolveCache( ActionExecutingContext filterContext )
    {

      var result = ResolveCache( filterContext, filterContext.ActionDescriptor, filterContext.ActionParameters );

      if ( result != null )
      {
        filterContext.Result = result;
        filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "OutputCache hitted" );
        filterContext.RouteData.DataTokens[CacheHitToken] = "Hitted";
      }
      else
        filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache COntrol", "OutputCache missed" );

      ;
    }


    protected virtual ActionResult ResolveCache( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      var policy = CreateCachePolicy( context, action, parameters );

      if ( policy == null )
        return null;

      var cachable = policy as IClientCacheablePolicy;//尝试输出客户端缓存
      if ( cachable != null )
      {
        if ( cachable.ResolveClientCache( context.HttpContext ) )
        {
          context.RouteData.DataTokens[ClientCacheResolved] = "Resolved";
          return new EmptyResult();
        }
      }


      //输出缓存
      var response = context.HttpContext.Cache.GetCachedResponse( policy.CacheToken );

      if ( response != null )
        return response.ToCachedResult();


      return null;
    }



    /// <summary>
    /// 创建缓存策略
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected abstract CachePolicy CreateCachePolicy( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters );






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


      var cachePolicy = dataTokens[CachePolicyToken] as IClientCacheablePolicy;

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




    protected virtual void UpdateCache( ResultExecutedContext filterContext )
    {
      //若已命中缓存，则不再更新缓存
      if ( filterContext.RouteData.DataTokens[CacheHitToken] != null )
        return;

      var result = filterContext.Result;
      var cachePolicy = filterContext.RouteData.DataTokens[CachePolicyToken] as CachePolicy;
      if ( cachePolicy == null )
        return;

      var mvcCachePolicy = cachePolicy as IMvcCachePolicy;
      if ( mvcCachePolicy != null )
      {
        UpdateCache( filterContext, result, mvcCachePolicy );
        return;
      }




      //对于子请求不予缓存。
      if ( filterContext.IsChildAction )
        return;

      var cachedResponse = GetCachedResponse( result );
      if ( cachedResponse == null )
        return;

      UpdateCache( cachedResponse, filterContext, cachePolicy );
    }

    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="filterContext"></param>
    /// <param name="result"></param>
    /// <param name="mvcCachePolicy"></param>
    protected virtual void UpdateCache( ControllerContext context, ActionResult result, IMvcCachePolicy mvcCachePolicy )
    {
      var cacheItem = mvcCachePolicy.CreateCacheItem( context, result );

      if ( cacheItem != null )
        context.HttpContext.Cache.InsertCacheItem( cacheItem );
    }



    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="cachedResponse">可被缓存的响应数据</param>
    /// <param name="context">MVC 请求上下文</param>
    /// <param name="policy">缓存策略</param>
    protected virtual void UpdateCache( ICachedResponse cachedResponse, ControllerContext context, CachePolicy policy )
    {
      var cacheItem = policy.CreateCacheItem( cachedResponse );
      if ( cacheItem != null )
        context.HttpContext.Cache.InsertCacheItem( cacheItem );
    }







    /// <summary>
    /// 从执行结果中获取可被缓存的响应数据
    /// </summary>
    /// <param name="result">Action 执行结果</param>
    /// <returns>可被缓存的响应数据，失败则返回 null</returns>
    protected virtual ICachedResponse GetCachedResponse( ActionResult result )
    {

      var cachable = result as ICacheableResult;
      if ( cachable != null )
        return cachable.GetCachedResponse();

      var viewResult = result as ViewResultBase;
      if ( viewResult != null )
      {
        cachable = viewResult.View as ICacheableResult;
        if ( cachable != null )
          return cachable.GetCachedResponse();
      }

      var content = result as ContentResult;
      if ( content != null )
        return CreateCachedResponse( content );

      return null;

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
