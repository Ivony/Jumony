using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  /// <summary>
  /// 用于指定 Action 或 Controller 应缓存输出结果。
  /// </summary>
  public class HtmlCachableAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting( ActionExecutingContext filterContext )
    {

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
    /// 获取缓存策略提供程序
    /// </summary>
    protected IHtmlCachePolicyProvider CachePolicyProvider
    {
      get;
      set;
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

      cacheKey = HtmlProviders.GetCacheKey( httpContext );
      if ( cacheKey == null )
        return;

      cachePolicy = HtmlProviders.GetCachePolicy( httpContext, cached );


      UpdateCache( cached, cacheKey, cachePolicy );
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

      var view = result as ViewResult;
      cachable = view.View as ICachableResult;
      if ( cachable != null )
        return cachable.GetCachedResponse();

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
