using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Web;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 输出缓存筛选器
  /// </summary>
  public abstract class CacheFilterBase : ActionFilterAttribute
  {

    /// <summary>
    /// 重写此方法以输出缓存
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {
      ResolveCache( filterContext );
    }


    private static readonly string CachePolicyToken = "JumonyforMVC_CacheControl_CachePolicy";
    private static readonly string CacheHitToken = "JumonyforMVC_CacheControl_CacheHit";
    private static readonly string ClientCacheResolved = "JumonyforMVC_CacheControl_ClientCacheResolved";


    /// <summary>
    /// 尝试输出缓存
    /// </summary>
    /// <param name="filterContext"></param>
    protected virtual void ResolveCache( ActionExecutingContext filterContext )
    {
      filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "Resolve Cache" );


      var result = ResolveCache( filterContext, filterContext.ActionDescriptor, filterContext.ActionParameters );

      if ( result != null )
      {
        filterContext.Result = result;
        filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "OutputCache hitted" );
        filterContext.RouteData.DataTokens[CacheHitToken] = "Hitted";
      }
      else
        filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "OutputCache missed" );

    }


    /// <summary>
    /// 尝试输出缓存
    /// </summary>
    /// <param name="context">当前请求上下文</param>
    /// <param name="action">当前请求的 Action</param>
    /// <param name="parameters">Action 的参数值</param>
    /// <returns>若可以缓存输出，则返回输出缓存的 ActionResult</returns>
    protected virtual ActionResult ResolveCache( ControllerContext context, ActionDescriptor action, IDictionary<string, object> parameters )
    {
      var policy = CreateCachePolicy( context, action, parameters );

      if ( policy == null )
        return null;


      context.RouteData.DataTokens[CachePolicyToken] = policy;


      var cachable = policy as IClientCacheablePolicy;//尝试输出客户端缓存
      if ( cachable != null && !context.IsChildAction )//对于子请求不尝试客户端缓存。
      {
        var _response = cachable.ResolveClientCache();
        if ( _response != null )
        {
          context.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "Resolve Client Cache Success" );

          context.RouteData.DataTokens[ClientCacheResolved] = "Resolved";
          return _response.ToCachedResult();
        }
      }


      //输出缓存
      var response = policy.ResolveCache();

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





    /// <summary>
    /// 重写 OnResultExecuting 方法以应用客户端缓存策略
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    public override void OnResultExecuting( ResultExecutingContext filterContext )
    {

      if ( !filterContext.IsChildAction )//对于子请求不执行客户端缓存策略。
        ApplyClientCachePolicy( filterContext );

    }


    /// <summary>
    /// 在输出任何内容前，应用客户端缓存策略
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
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
      if ( cachePolicy == null )
        return;

      filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "Apply Client Cache Policy" );

      cachePolicy.ApplyClientCachePolicy();
    }

    /// <summary>
    /// 重写此方法使得操作结果执行完毕后，更新缓存信息
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnResultExecuted( ResultExecutedContext filterContext )
    {
      UpdateCache( filterContext );
    }



    /// <summary>
    /// 更新缓存
    /// </summary>
    /// <param name="filterContext">筛选器上下文</param>
    protected virtual void UpdateCache( ResultExecutedContext filterContext )
    {
      //若已命中缓存，则不再更新缓存
      if ( filterContext.RouteData.DataTokens[CacheHitToken] != null )
        return;

      var result = filterContext.Result;
      var cachePolicy = filterContext.RouteData.DataTokens[CachePolicyToken] as CachePolicy;
      if ( cachePolicy == null )
        return;


      filterContext.HttpContext.Trace.Write( "Jumony for MVC - Cache Control", "Update Cache" );


      var mvcCachePolicy = cachePolicy as IMvcCachePolicy;
      if ( mvcCachePolicy != null )
      {
        UpdateCache( filterContext, result, mvcCachePolicy );
        return;
      }



      var cachedResponse = GetCachedResponse( result );
      if ( cachedResponse == null )
        return;

      UpdateCache( cachedResponse, filterContext, cachePolicy );
    }

    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="result">Action执行结果</param>
    /// <param name="mvcCachePolicy">MVC 缓存策略</param>
    protected virtual void UpdateCache( ControllerContext context, ActionResult result, IMvcCachePolicy mvcCachePolicy )
    {
      mvcCachePolicy.UpdateCache( context, result );
    }



    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="cachedResponse">可被缓存的响应数据</param>
    /// <param name="context">MVC 请求上下文</param>
    /// <param name="policy">缓存策略</param>
    protected virtual void UpdateCache( ICachedResponse cachedResponse, ControllerContext context, CachePolicy policy )
    {

      policy.UpdateCache( cachedResponse );
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
