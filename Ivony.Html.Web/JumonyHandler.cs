using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Ivony.Fluent;
using System.Web.Hosting;

using Ivony.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// Jumony 用于处理 HTTP 请求的处理程序
  /// </summary>
  public class JumonyHandler : IHttpHandler, IRequiresSessionState
  {

    /// <summary>
    /// 指定此实例是否可以被复用，默认值为false
    /// </summary>
    public virtual bool IsReusable
    {
      get { return false; }
    }


    /// <summary>
    /// 实现 IHttpHandler.ProcessRequest
    /// </summary>
    /// <param name="context">当前 HTTP 请求的上下文</param>
    void IHttpHandler.ProcessRequest( HttpContext context )
    {

      ProcessRequest( new HttpContextWrapper( context ) );

    }



    /// <summary>
    /// 获取当前请求上下文
    /// </summary>
    protected virtual RequestContext RequestContext
    {
      get { return HttpContext.Request.RequestContext; }
    }




    private HttpContextBase _httpContext;

    /// <summary>
    /// 获取当前 HTTP 请求上下文
    /// </summary>
    protected virtual HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }






    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文信息</param>
    protected void ProcessRequest( HttpContextBase context )
    {

      _httpContext = context;

      Trace.Write( "Jumony Web", "Begin of Request" );

      if ( RequestContext == null )
        throw DirectVisitError();


      if ( RequestContext.RouteData == null || !(RequestContext.RouteData.Route is IHtmlRequestRoute) )
      {
        Trace.Write( "Jumony Web", "Request Error: route type error." );
        throw DirectVisitError();
      }



      var virtualPath = RequestContext.RouteData.DataTokens[JumonyRequestRoute.VirtualPathToken] as string;
      RequestContext.RouteData.DataTokens.Remove( JumonyRequestRoute.VirtualPathToken );

      virtualPath = virtualPath ?? RequestContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath;


      var response = ProcessRequest( virtualPath );
      OutputResponse( RequestContext.HttpContext, response );


      RequestContext.HttpContext.Trace.Write( "Jumony Web", "End of Request" );
    }




    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="virtualPath">当前要处理的虚拟路径</param>
    protected virtual ICachedResponse ProcessRequest( string virtualPath )
    {
      ICachedResponse response;

      Trace.Write( "Jumony Web", "Begin resolve cache." );
      response = ResolveCache( virtualPath );
      Trace.Write( "Jumony Web", "End resolve cache" );

      if ( response == null )
      {

        response = ProcessRequest( new HtmlRequestContext( HttpContext, virtualPath, LoadDocument( virtualPath ) ), GetHandler( virtualPath ) );


        Trace.Write( "Jumony Web", "Begin update cache" );
        UpdateCache( response );
        Trace.Write( "Jumony Web", "End update cache." );

      }

      else
        Trace.Write( "Jumony Web", "Cache resolved." );

      return response;
    }





    /// <summary>
    /// 派生类重写此方法接管 HTTP 请求处理流程
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    /// <param name="handler">用于处理 HTML 文档的处理程序</param>
    /// <returns>处理后的结果</returns>
    protected virtual ICachedResponse ProcessRequest( HtmlRequestContext context, IHtmlHandler handler )
    {

      var filters = GetFilters( context.VirtualPath );



      OnProcessing( context, filters );

      Trace.Write( "Jumony Web", "Begin process document." );
      handler.ProcessScope( context );
      Trace.Write( "Jumony Web", "End process document." );

      OnProcessed( context, filters );




      OnRendering( context, filters );

      Trace.Write( "Jumony Web", "Begin render document." );
      string content;
      using ( StringWriter writer = new StringWriter() )
      {
        context.Scope.RenderChilds( writer, GetAdapters( handler ) );
        content = writer.ToString();
      }

      Trace.Write( "Jumony Web", "End render document." );

      OnRendered( context, filters );

      return CreateResponse( content );
    }



    /// <summary>
    /// 派生类重写此方法自定义加载文档的逻辑
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>加载的文档对象</returns>
    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      IHtmlDocument document;

      Trace.Write( "Jumony Web", "Begin load document." );


      document = HtmlServices.LoadDocument( virtualPath );
      if ( document == null )
        throw new HttpException( 404, "加载文档失败" );


      Trace.Write( "Jumony Web", "End load document." );

      return document;
    }






    /// <summary>
    /// 获取 HTML 处理程序
    /// </summary>
    /// <param name="virtualPath">要处理的 HTML 文档的虚拟路径</param>
    /// <returns>HTML 处理程序</returns>
    protected virtual IHtmlHandler GetHandler( string virtualPath )
    {

      IHtmlHandler handler = null;

      if ( RequestContext != null )
      {
        handler = RequestContext.RouteData.DataTokens[JumonyRequestRoute.HtmlHandlerToken] as IHtmlHandler;
        RequestContext.RouteData.DataTokens.Remove( JumonyRequestRoute.HtmlHandlerToken );
      }

      return handler ?? HtmlHandlerProvider.GetHandler( virtualPath );
    }




    /// <summary>
    /// 产生一个异常，用于说明 HTML 处理程序不能直接访问
    /// </summary>
    /// <returns>HTTP 404 异常</returns>
    public static Exception DirectVisitError()
    {
      return new HttpException( 404, "不能直接访问 Jumony 页处理程序。" );
    }


    /// <summary>
    /// 获取用于写入追踪信息的上下文
    /// </summary>
    protected virtual TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }


    /// <summary>
    /// 获取当前适用的渲染代理
    /// </summary>
    /// <returns>要用于当前渲染过程的渲染代理</returns>
    protected virtual IHtmlRenderAdapter[] GetAdapters( object handler )
    {
      return new IHtmlRenderAdapter[] { new PartialRenderAdapter( HttpContext, handler ) };
    }





    /// <summary>
    /// 获取 HTML 筛选器
    /// </summary>
    /// <param name="virtualPath">HTML 文档虚拟路径</param>
    /// <returns>HTML 筛选器</returns>
    protected virtual IHtmlFilter[] GetFilters( string virtualPath )
    {
      return WebServiceLocator
        .GetServices<IHtmlFilterProvider>( virtualPath )
        .SelectMany( p => p.GetFilters() )
        .Reverse().ToArray();
    }




    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    protected CachePolicy CachePolicy
    {
      get;
      private set;
    }

    /// <summary>
    /// 尝试获取缓存的输出
    /// </summary>
    /// <returns>缓存的输出</returns>
    protected virtual ICachedResponse ResolveCache( string virtualPath )
    {
      var policy = HtmlServices.GetCachePolicy( HttpContext );

      if ( policy == null )
        return null;


      CachePolicy = policy;

      var clientCachePolicy = policy as IClientCacheablePolicy;

      if ( clientCachePolicy != null )
      {

        var response = clientCachePolicy.ResolveClientCache();
        if ( response != null )
          return response;

      }

      return CachePolicy.ResolveCache();
    }


    /// <summary>
    /// 刷新输出缓存
    /// </summary>
    /// <param name="cachedResponse">响应的缓存</param>
    protected virtual void UpdateCache( ICachedResponse cachedResponse )
    {

      if ( CachePolicy == null )
        return;


      CachePolicy.UpdateCache( cachedResponse );

    }


    /// <summary>
    /// 派生类重写此方法自定义创建响应的逻辑
    /// </summary>
    /// <param name="content">响应内容</param>
    /// <returns>响应</returns>
    protected virtual ICachedResponse CreateResponse( string content )
    {
      return new RawResponse() { Content = content };
    }



    /// <summary>
    /// 派生类重写此方法自定义输出响应的逻辑
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <param name="responseData">响应信息</param>
    protected virtual void OutputResponse( HttpContextBase context, ICachedResponse responseData )
    {
      responseData.Apply( context.Response );
    }


    /// <summary>
    /// 这个方法是用来添加<![CDATA[<meta name="generator" value="jumony" />]]>元素的。
    /// </summary>
    private void AddGeneratorMetaData( IHtmlDocument document )
    {
      var modifier = document.DomModifier;
      if ( modifier != null )
      {
        var header = document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
        {

          var metaElement = modifier.AddElement( header, "meta" );

          metaElement.SetAttribute( "name", "generator" );
          metaElement.SetAttribute( "content", "Jumony" );
        }
      }
    }




    /// <summary>引发 Processing 事件</summary>
    protected virtual void OnProcessing( HtmlRequestContext context, IHtmlFilter[] filters )
    {
      foreach ( var filter in filters )
      {
        try
        {
          filter.OnProcessing( context );
        }
        catch { }
      }
    }

    /// <summary>引发 Processed 事件</summary>
    protected virtual void OnProcessed( HtmlRequestContext context, IHtmlFilter[] filters )
    {
      foreach ( var filter in filters.Reverse() )
      {
        try
        {
          filter.OnProcessed( context );
        }
        catch { }
      }
    }


    /// <summary>引发 Rendering 事件</summary>
    protected virtual void OnRendering( HtmlRequestContext context, IHtmlFilter[] filters )
    {
      foreach ( var filter in filters )
      {
        try
        {
          filter.OnRendering( context );
        }
        catch { }
      }
    }

    /// <summary>引发 Rendered 事件</summary>
    protected virtual void OnRendered( HtmlRequestContext context, IHtmlFilter[] filters )
    {
      foreach ( var filter in filters.Reverse() )
      {
        try
        {
          filter.OnRendered( context );
        }
        catch { }
      }
    }




    /// <summary>
    /// 执行与释放或重置非托管资源相关的应用程序定义的任务
    /// </summary>
    public virtual void Dispose()
    {

    }

  }
}
