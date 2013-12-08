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
  /// Jumony 用于处理 HTTP 请求的处理器
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

      ProcessRequest( context.Request.RequestContext );

    }

    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文信息</param>
    protected void ProcessRequest( RequestContext context )
    {

      context.HttpContext.Trace.Write( "Jumony Web", "Begin of Request" );

      if ( context.RouteData == null || !(context.RouteData.Route is IHtmlRequestRoute) )
        throw DirectVisitError();

      var virtualPath = context.RouteData.DataTokens[JumonyRequestRoute.VirtualPathToken] as string;
      virtualPath = virtualPath ?? context.HttpContext.Request.AppRelativeCurrentExecutionFilePath;


      var response = ProcessRequest( context.HttpContext, virtualPath, false );
      OutputResponse( context.HttpContext, response );

      context.HttpContext.Trace.Write( "Jumony Web", "End of Request" );
    }


    public virtual string ProcessPartial( HttpContextBase context, string virtualPath )
    {

      var response = ProcessRequest( context, virtualPath, true );
      return response.CastTo<PartialResponse>().Content;

    }



    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 请求上下文</param>
    /// <param name="virtualPath">当前要处理的虚拟路径</param>
    /// <param name="isPartial">是否为部分视图模式</param>
    protected virtual ICachedResponse ProcessRequest( HttpContextBase context, string virtualPath, bool isPartial )
    {
      _httpContext = context;

      ICachedResponse response;

      Trace.Write( "Jumony Web", "Begin resolve cache." );
      response = ResolveCache();
      Trace.Write( "Jumony Web", "End resolve cache" );

      if ( response == null )
      {

        response = ProcessRequestCore( context, virtualPath, isPartial );


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
    /// <param name="httpContext">HTTP 请求上下文</param>
    /// <param name="virtualPath">当前请求文档的虚拟路径</param>
    /// <returns>处理后的结果</returns>
    protected virtual ICachedResponse ProcessRequestCore( HttpContextBase httpContext, string virtualPath, bool isPartial )
    {

      IHtmlDocument document;

      OnPreLoadDocument();

      Trace.Write( "Jumony Web", "Begin load document." );
      document = LoadDocument( virtualPath );
      Trace.Write( "Jumony Web", "End load document." );

      OnPostLoadDocument();


      var filters = InitailizeFilters( virtualPath, document );
      var handler = GetHandler( virtualPath, document );



      HtmlRequestContext context;
      if ( isPartial )
        context = new HtmlRequestContext( httpContext, virtualPath, GetPartialScope( document ) );

      else
        context = new HtmlRequestContext( httpContext, virtualPath, document );

      OnPreProcessDocument();

      Trace.Write( "Jumony Web", "Begin process document." );
      handler.ProcessScope( context );
      Trace.Write( "Jumony Web", "End process document." );

      OnPostProcessDocument();




      OnPreRender();

      Trace.Write( "Jumony Web", "Begin render document." );
      string content;
      using ( StringWriter writer = new StringWriter() )
      {
        context.Scope.RenderChilds( writer, GetAdapters() );
        content = writer.ToString();
      }

      Trace.Write( "Jumony Web", "End render document." );

      OnPostRender();

      if ( isPartial )
        return CreatePartialResponse( content );
      else
        return CreateResponse( content );
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
    /// 获取部分视图要处理的范围
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    protected virtual IHtmlContainer GetPartialScope( IHtmlDocument document )
    {
      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }



    /// <summary>
    /// 获取当前适用的渲染代理
    /// </summary>
    /// <returns>要用于当前渲染过程的渲染代理</returns>
    protected virtual IHtmlRenderAdapter[] GetAdapters()
    {
      return new IHtmlRenderAdapter[0];
    }


    /// <summary>
    /// 派生类重写此方法自定义加载文档的逻辑
    /// </summary>
    /// <param name="virtualPath">文档的虚拟路径</param>
    /// <returns>加载的文档对象</returns>
    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      return HtmlServices.LoadDocument( virtualPath );
    }

    /// <summary>
    /// 获取 HTML 处理程序
    /// </summary>
    /// <param name="virtualPath">HTML 文档虚拟路径</param>
    /// <param name="document">HTML 文档</param>
    /// <returns>HTML 处理程序</returns>
    protected virtual IHtmlHandler GetHandler( string virtualPath, IHtmlDocument document )
    {
      var services = WebServiceLocator.GetServices<IHtmlHandlerProvider>( virtualPath ).Concat( new[] { DefaultProviders.GetHtmlHandlerProvider() } );
      foreach ( var provider in services )
      {
        var handler = provider.GetHandler( virtualPath );

        if ( handler != null )
          return handler;

      }

      return new HtmlHandler();
    }


    /// <summary>
    /// 获取 HTML 筛选器
    /// </summary>
    /// <param name="virtualPath">HTML 文档虚拟路径</param>
    /// <param name="document">HTML 文档</param>
    /// <returns>HTML 筛选器</returns>
    protected virtual IHtmlFilter[] InitailizeFilters( string virtualPath, IHtmlDocument document )
    {
      return WebServiceLocator.GetServices<IHtmlFilterProvider>( virtualPath ).SelectMany( p => p.GetFilters( virtualPath, document ) ).ToArray();
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
    protected virtual ICachedResponse ResolveCache()
    {
      var policy = HtmlServices.GetCachePolicy( HttpContext );

      if ( policy == null )
        return null;


      CachePolicy = policy;

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
    /// 派生类重写此方法自定义创建部分视图响应的逻辑
    /// </summary>
    /// <param name="content">响应内容</param>
    /// <returns>响应</returns>
    protected virtual ICachedResponse CreatePartialResponse( string content )
    {
      return new PartialResponse() { Content = content };
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



    private HttpContextBase _httpContext;

    /// <summary>
    /// 获取与该页关联的 HttpContext 对象。
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }



    /// <summary>在加载文档前引发此事件</summary>
    public event EventHandler PreLoadDocument;
    /// <summary>在加载文档后引发此事件</summary>
    public event EventHandler PostLoadDocument;

    /// <summary>引发 PreLoadDocument 事件</summary>
    protected virtual void OnPreLoadDocument() { if ( PreLoadDocument != null ) PreLoadDocument( this, EventArgs.Empty ); }
    /// <summary>引发 PostLoadDocument 事件</summary>
    protected virtual void OnPostLoadDocument() { if ( PostLoadDocument != null ) PostLoadDocument( this, EventArgs.Empty ); }


    /// <summary>在处理文档前引发此事件</summary>
    public event EventHandler PreProcessDocument;
    /// <summary>在处理文档后引发此事件</summary>
    public event EventHandler PostProcessDocument;

    /// <summary>引发 PreProcessDocument 事件</summary>
    protected virtual void OnPreProcessDocument()
    {
      if ( PreProcessDocument != null ) PreProcessDocument( this, EventArgs.Empty );
    }
    /// <summary>引发 PostProcessDocument 事件</summary>
    protected virtual void OnPostProcessDocument() { if ( PostProcessDocument != null ) PostProcessDocument( this, EventArgs.Empty ); }


    /// <summary>在渲染文档前引发此事件</summary>
    public event EventHandler PreRender;
    /// <summary>在渲染文档后引发此事件</summary>
    public event EventHandler PostRender;

    /// <summary>引发 PreRender 事件</summary>
    protected virtual void OnPreRender() { if ( PreRender != null ) PreRender( this, EventArgs.Empty ); }
    /// <summary>引发 PostRender 事件</summary>
    protected virtual void OnPostRender() { if ( PostRender != null ) PostRender( this, EventArgs.Empty ); }




    /// <summary>
    /// 执行与释放或重置非托管资源相关的应用程序定义的任务
    /// </summary>
    public virtual void Dispose()
    {

    }

  }
}
